using System.ComponentModel.DataAnnotations;

namespace App.Domain.Configuration.Web
{
    public sealed class WebConfigurationElement
    {
        /// <summary>
        /// Web endpoing url
        /// </summary>
        [Required]
        public string WebUrl { get; set; }

        /// <summary>
        /// Web endpoing url
        /// </summary>
        [Required]
        public string ApiUrl { get; set; }

        /// <summary>
        /// The domain of the cookie
        /// </summary>
        [Required]
        public string CookieDomain { get; set; }
    }
}