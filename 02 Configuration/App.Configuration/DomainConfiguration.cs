using App.Configuration.Application;
using App.Configuration.Database;
using App.Configuration.Localization;
using App.Configuration.Notifications;
using App.Configuration.Web;
using Signals.Aspects.Configuration;

namespace App.Configuration
{
    public class DomainConfiguration : BaseConfiguration<DomainConfiguration>
    {
        /// <summary>
        /// Represents the confgiruation section name of the custom configuration
        /// </summary>
        public override string Key => nameof(DomainConfiguration);

        /// <summary>
        /// Configuration for the application itself
        /// </summary>
        public ApplicationConfigurationElement ApplicationConfiguration { get; set; }

        /// <summary>
        /// Configuration for notification
        /// </summary>
        public NotificationConfigurationElement NotificationConfiguration { get; set; }
    
        /// <summary>
        /// Configuration for web
        /// </summary>
        public WebConfigurationElement WebConfiguration { get; set; }

        /// <summary>
        /// Configuration for database
        /// </summary>
        public DatabaseConfigurationElement DatabaseConfiguration { get; set; }

        /// <summary>
        /// Localization configuration
        /// </summary>
        public LocalizationConfigurationElement LocalizationConfiguration { get; set; }
    }
}