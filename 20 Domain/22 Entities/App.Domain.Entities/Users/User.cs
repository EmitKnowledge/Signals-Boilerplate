using System.Runtime.Serialization;
using System;
using App.Domain.Entities.Companies;
using App.Domain.Entities.Base;

namespace App.Domain.Entities.Users
{
    /// <summary>
    /// Represent the client/customer user in the system
    /// </summary>
    [Serializable]
    [DataContract]
    public class User : BaseDomainEntity, IBaseCompanyDomainEntity
    {
        /// <summary>
        /// Thje company id to which this user belongs to
        /// </summary>
        [DataMember]
        public int CompanyId { get; set; }

        /// <summary>
        /// Thje company to which this user belongs to
        /// </summary>
        [DataMember]
        public Company Company { get; set; }

        /// <summary>
        /// Email of the user
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Username of the user
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// Name (First name and Last name) of the user
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// User's hashed pass
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// Salt used for the encrypting the password
        /// </summary>
        [DataMember]
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Indicates that the user is verified after registration
        /// </summary>
        [DataMember]
        public bool IsVerified { get; set; }

        /// <summary>
        /// Represents the type of the user
        /// </summary>
        [DataMember]
        public UserType Type { get; set; }
    }
}