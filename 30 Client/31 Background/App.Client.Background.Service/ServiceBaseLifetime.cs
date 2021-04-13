using System.Threading.Tasks;
using System.Threading;
using System.ServiceProcess;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace App.Client.Background.Service
{
    /// <summary>
    /// Extensions for hosting lifetime
    /// </summary>
    public static class ServiceBaseLifetimeHostExtensions
    {
        /// <summary>
        /// Lifetime configuration
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IHostBuilder UseServiceBaseLifetime(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) => services.AddSingleton<IHostLifetime, ServiceBaseLifetime>());
        }

        /// <summary>
        /// Extension for running as windows service
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsServiceAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken = default(CancellationToken))
        {
            return hostBuilder.UseServiceBaseLifetime().Build().RunAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Application hosting lifetime configration
    /// </summary>
    public class ServiceBaseLifetime : ServiceBase, IHostLifetime
    {
        private readonly TaskCompletionSource<object> _delayStart = new TaskCompletionSource<object>();

        private IHostApplicationLifetime ApplicationLifetime { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="applicationLifetime"></param>
        public ServiceBaseLifetime(IHostApplicationLifetime applicationLifetime)
        {
            ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        }

        /// <summary>
        /// Handle hor waiting for task
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _delayStart.TrySetCanceled());
            ApplicationLifetime.ApplicationStopping.Register(Stop);

            new Thread(Run).Start(); // Otherwise this would block and prevent IHost.StartAsync from finishing.
            return _delayStart.Task;
        }

        private void Run()
        {
            try
            {
                Run(this); // This blocks until the service is stopped.
                _delayStart.TrySetException(new InvalidOperationException("Stopped without starting"));
            }
            catch (Exception ex)
            {
                _delayStart.TrySetException(ex);
            }
        }

        /// <summary>
        /// Handle for stopping task
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Stop();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by base.Run when the service is ready to start.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            _delayStart.TrySetResult(null);
            base.OnStart(args);
        }

        /// <summary>
        /// Called by base.Stop. This may be called multiple times by service Stop, ApplicationStopping, and StopAsync.
        /// That's OK because StopApplication uses a CancellationTokenSource and prevents any recursion.
        /// </summary>
        protected override void OnStop()
        {
            ApplicationLifetime.StopApplication();
            base.OnStop();
        }
    }
}