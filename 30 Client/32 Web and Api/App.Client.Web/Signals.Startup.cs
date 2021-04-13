using Signals.Aspects.ErrorHandling.Polly;
using Signals.Aspects.CommunicationChannels.MsSql.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System;
using Signals.Core.Web.Extensions;
using Signals.Core.Web.Configuration;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Configuration;
using Signals.Core.Common.Instance;
using Signals.Aspects.Logging.NLog.Configurations;
using Signals.Aspects.Logging.Enums;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Aspects.DI.Autofac;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.Caching.InMemory;
using Signals.Aspects.Caching.Enums;
using Signals.Aspects.Auth.NetCore.Extensions;
using NodaTime.Serialization.JsonNet;
using NodaTime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using Autofac.Extensions.DependencyInjection;
using App.Domain.Configuration;

namespace App.Client.Web
{
    /// <summary>
    /// Signals bootstrapping class
    /// </summary>
    public static class SignalsStartup
    {
        /// <summary>
        /// Add signals aspects
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceProvider AddSignals(this IServiceCollection services)
        {
            // prepare referenced dll's for registration in the registration service
            var registrationService = new RegistrationService();
            var assemblies = Directory.GetFiles(AppContext.BaseDirectory, "*.dll").Select(Assembly.LoadFrom).ToList();

            // load custom configurations from file and set active configuration
            services.AddConfiguration();
            // use default memory cache
            services.AddMemoryCache();
            services.AddAuthentication("Cookies")
                    .AddSignalsAuth(opts =>
                    {
                        opts.Cookie.IsEssential = true;
                        opts.Cookie.MaxAge = TimeSpan.FromDays(365);
                        //opts.Cookie.Domain = DomainConfiguration.Instance.WebConfiguration.CookieDomain;

                        // if you are using CORS, make sure cookie policies are properly configuired
                        //opts.Cookie.SameSite = SameSiteMode.None;
                        //opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                        opts.Events = new CookieAuthenticationEvents
                        {
                            OnRedirectToLogin = context =>
                            {
                                context.HttpContext.Response.Redirect($"{WebApplicationConfiguration.Instance.WebUrl}/login");
                                return Task.CompletedTask;
                            },
                            OnSigningOut = context =>
                            {
                                context.CookieOptions.SameSite = SameSiteMode.Lax;
                                return Task.CompletedTask;
                            }
                        };
                    });

            // set data protection
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, @"..\persisted_keys")))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365))
                .SetApplicationName(ApplicationConfiguration.Instance.ApplicationName);

            // populate the service with referenced dlls
            registrationService.Builder.Populate(services);

            // configure Signals aspects
            services
                .AddSignals(config =>
                {
                    // configure dependency injection
                    config.ScanAssemblies = assemblies;
                    config.RegistrationService = registrationService;
                    // configure caching 
                    config.CacheConfiguration = new InMemoryCacheConfiguration
                    {
                        DataProvider = new InMemoryDataProvider(),
                        ExpirationPolicy = CacheExpirationPolicy.Sliding,
                        ExpirationTime = TimeSpan.FromMinutes(1)
                    };
                    // configure logging in database
                    config.LoggerConfiguration = new DatabaseLoggingConfiguration
                    {
                        Database = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.Database,
                        Host = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.IpAddress,
                        Username = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.Uid,
                        Password = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.Pwd,
                        DataProvider = DataProvider.SqlClient,
                        TableName = "LogEntity"
                    };
                    // configure localization from json files
                    config.LocalizationConfiguration = new JsonDataProviderConfiguration
                    {
                        DirectoryPath = Path.Combine(AppContext.BaseDirectory, "system.resources"),
                        FileExtension = "app",
                        LocalizationSources = new List<LocalizationSource>
                        {
                            new LocalizationSource
                            {
                                Name = "Mail messages",
                                SourcePath = "mailmessages"
                            },
                            new LocalizationSource
                            {
                                Name = "Validation rules",
                                SourcePath = "validationrules"
                            },
                            new LocalizationSource
                            {
                                Name = "Pages",
                                SourcePath = "pages"
                            },
                            new LocalizationSource
                            {
                                Name = "Processes",
                                SourcePath = "processes"
                            }
                        },
                    };
                    // configure pubsub with MSSQL broker
                    config.ChannelConfiguration = new MsSqlChannelConfiguration
                    {
                        // make sure broker is enabled in the database
                        // ex: ALTER DATABASE [acme.db] SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE
                        ConnectionString = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.ConnectionString,
                        DbTableName = DomainConfiguration.Instance.NotificationConfiguration.DbTableName,
                        MessageListeningStrategy = MessageListeningStrategy.Broker
                    };
                    config.StrategyBuilder = new StrategyBuilder().SetAutoHandling(false);
                    config.ConfigureJsonSerialization();
                });

            // wrap autofac container
            return new AutofacServiceProvider(registrationService.ServiceContainer.Container);
        }

        /// <summary>
        /// Load configuration from files
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static void AddConfiguration(this IServiceCollection services)
        {
            string environment = null;
            FileConfigurationProvider ProviderForFile(string name) => new FileConfigurationProvider
            {
                File = name,
                Path = environment.IsNullOrEmpty() ? Path.Combine(AppContext.BaseDirectory, $"configs") : Path.Combine(AppContext.BaseDirectory, $"configs", environment),
                ReloadOnAccess = false
            };

            // set active configuration
            EnvironmentConfiguration.UseProvider(ProviderForFile("environment.config.json"));
            environment = EnvironmentConfiguration.Instance.Environment;

            // load custom configurations from file

            // Signals core application configuration
            ApplicationConfiguration.UseProvider(ProviderForFile("application.config.json"));
            // Signals core web application configuration
            WebApplicationConfiguration.UseProvider(ProviderForFile("web.application.config.json"));
            // Application custom domain configuration
            DomainConfiguration.UseProvider(ProviderForFile("domain.config.json"));
        }

        /// <summary>
        /// Set default serialization settings
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static void ConfigureJsonSerialization(this ApplicationBootstrapConfiguration config)
        {
            config.JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
            config.JsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        }
    }
}