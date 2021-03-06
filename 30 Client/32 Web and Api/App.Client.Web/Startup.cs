using App.Domain.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Signals.Aspects.Auth.NetCore.Extensions;
using Signals.Core.Web.Extensions;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace App.Client.Web
{
    /// <summary>
    /// Statrtup configuration
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Application configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            return services.AddSignals();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // show exception for dev environament
            if (EnvironmentConfiguration.IsDevelopment)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            // set redirect to https as default
            app.UseHttpsRedirection();

            app.Use((HttpContext context, Func<Task> next) =>
            {
                var syncIOFeature = context.Features.Get<IHttpBodyControlFeature>();
                if (syncIOFeature != null)
                {
                    syncIOFeature.AllowSynchronousIO = true;
                }

                var cookieKey = DomainConfiguration.Instance.LocalizationConfiguration.CookieKey;
                var defaultCulture = DomainConfiguration.Instance.LocalizationConfiguration.DefaultCulture;

                if (context.Request.Cookies.ContainsKey(cookieKey))
                {
                    var cookie = context.Request.Cookies[cookieKey];
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(cookie);
                }
                else
                {
                    context.Response.Cookies.Append(cookieKey, defaultCulture);
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(defaultCulture);
                }

                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

                return next();
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/api/spec", $"{DomainConfiguration.Instance.ApplicationConfiguration.ApplicationName} API v1");
                options.RoutePrefix = "api-playground";
            });
            app.UseStaticFiles();
            app.UseSignalsAuth();
            app.UseSignals();
        }
    }
}