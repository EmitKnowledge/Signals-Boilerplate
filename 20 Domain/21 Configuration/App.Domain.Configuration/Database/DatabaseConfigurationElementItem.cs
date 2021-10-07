using System.ComponentModel.DataAnnotations;

namespace App.Domain.Configuration.Database
{
    /// <summary>
    /// Defines database configuration element
    /// </summary>
    public sealed class DatabaseConfigurationElementItem
    {
        /// <summary>
        /// Environment name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Server address
        /// </summary>
        [Required]
        public string IpAddress { get; set; }

        /// <summary>
        /// Connection string database
        /// </summary>
        [Required]
        public string Database { get; set; }

        /// <summary>
        /// Connection string user id, leave empty if TrustedConnection
        /// </summary>        
        public string Uid { get; set; }

        /// <summary>
        /// Connection string password, leave empty if TrustedConnection
        /// </summary>        
        public string Pwd { get; set; }

        /// <summary>
        /// Create valid SQL Server connection string
        /// </summary>
        public string ConnectionString
        {
            get
            {
                if(string.IsNullOrEmpty(Uid) && string.IsNullOrEmpty(Pwd))
                    return string.Format("server={0};Database={1};Trusted_Connection=True", IpAddress, Database);
                
                return string.Format("server={2};Database={3};User Id={0};Password = {1}", Uid, Pwd, IpAddress, Database);
            }
        }
    }
}