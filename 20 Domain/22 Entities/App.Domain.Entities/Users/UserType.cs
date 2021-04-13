using System;
using System.Runtime.Serialization;

namespace App.Domain.Entities.Users
{
    /// <summary>
    /// Represents the user type (Default roles)
    /// </summary>
    [DataContract]
    [Serializable]
    public enum UserType
    {
        /// <summary>
        /// Anonymous app user
        /// </summary>
        [EnumMember]
        Anonymous,

        /// <summary>
        /// Employee of the company
        /// </summary>
        [EnumMember]
        CompanyEmployee,

        /// <summary>
        /// Administrator of the company
        /// </summary>
        [EnumMember]
        CompanyAdmin,

        /// <summary>
        /// Administrator of the system
        /// </summary>
        [EnumMember]
        SystemAdmin
    }
}