using System;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Net;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using Signals.Core.Configuration;
using Signals.Core.Common.Instance;
using Signals.Core.Background.Configuration;
using Signals.Core.Background.Configuration.Bootstrapping;
using Signals.Aspects.Logging.NLog.Configurations;
using Signals.Aspects.Logging.Enums;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Aspects.ErrorHandling.Strategies;
using Signals.Aspects.ErrorHandling.Polly;
using Signals.Aspects.DI.Autofac;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.CommunicationChannels.MsSql.Configurations;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.Caching.Enums;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Microsoft.Extensions.Hosting;
using App.Domain.Configuration;
using Signals.Aspects.BackgroundProcessing.FluentScheduler;

namespace App.Client.Background.Service;

/// <summary>
    /// Background service hosting handle
    /// </summary>
    public class BackgroundServiceService : IHostedService, IDisposable
    {
        /// <summary>
        /// IDisposable implementation
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Called when process is starting
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

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
            // Signals core background application configuration
            BackgroundApplicationConfiguration.UseProvider(ProviderForFile("background.application.config.json"));
            // Application custom domain configuration
            DomainConfiguration.UseProvider(ProviderForFile("domain.config.json"));

            // use general exception handling strategy
            var strategyBuilder = new StrategyBuilder();
            strategyBuilder.Add<Exception>(new RetryStrategy { RetryCount = 3, RetryCooldown = TimeSpan.FromMinutes(5) }).SetAutoHandling(false);

            // configure Signals aspects
            var config = new BackgroundApplicationBootstrapConfiguration
            {
                // configure dependency injection
                RegistrationService = new RegistrationService(),
                // recurring task registry
                TaskRegistry = new FluentRegistry(),
                // configure caching 
                CacheConfiguration = new InMemoryCacheConfiguration
                {
                    ExpirationPolicy = CacheExpirationPolicy.Sliding,
                    ExpirationTime = TimeSpan.FromMinutes(1)
                },
                // configure logging in database
                LoggerConfiguration = new DatabaseLoggingConfiguration
                {
                    Database = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.Database,
                    Host = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.IpAddress,
                    Username = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.Uid,
                    Password = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.Pwd,
                    DataProvider = DataProvider.SqlClient,
                    TableName = "LogEntity"
                },
                // configure localization from json files
                LocalizationConfiguration = new JsonDataProviderConfiguration
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
                    }
                },
                // configure pubsub with MSSQL broker
                ChannelConfiguration = new MsSqlChannelConfiguration
                {
                    // make sure broker is enabled in the database
                    // ex: ALTER DATABASE [acme.db] SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE
                    ConnectionString = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.ConnectionString,
                    DbTableName = DomainConfiguration.Instance.NotificationConfiguration.DbTableName,
                    MessageListeningStrategy = MessageListeningStrategy.Broker
                },
                StrategyBuilder = new StrategyBuilder().SetAutoHandling(false),
            };

            // set default serialization settings
            config.JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
            config.JsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            // bootstrap the configuration from the references assemblies
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "App.*.dll").Select(file => Assembly.LoadFrom(file)).ToArray();
            config.Bootstrap(assemblies);

            // set current culture and UI culture
            Thread.CurrentThread.CurrentCulture = new CultureInfo(DomainConfiguration.Instance.LocalizationConfiguration.DefaultCulture);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when process is ending
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }