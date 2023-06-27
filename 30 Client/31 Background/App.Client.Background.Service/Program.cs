using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace App.Client.Background.Service
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services.AddHostedService<BackgroundServiceService>();
                });
        }
    }

    
}