using App.Domain.Entities.Users;
using Ganss.Xss;
using Signals.Core.Processing.Input;
using System;
using System.Runtime.Serialization;

namespace App.Client.Web.Endpoints.Users.Dtos
{
    [DataContract, Serializable]
    public class RegisterUserDto : IDtoData
    {
        /// <summary>
        /// Represents the user's company id
        /// </summary>
        [DataMember]
        public int CompanyId { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// User's password
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// User's name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// User's type
        /// </summary>
        [DataMember]
        public UserType Type { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
            Name = sanitizer.Sanitize(Name);
        }

        public static implicit operator User(RegisterUserDto u)
        {
            if (u == null) return null;
            return new User
            {
                CompanyId = u.CompanyId,
                Password = u.Password,
                Email = u.Email,
                Name = u.Name,
                Type = u.Type,
                IsVerified = false
            };
        }
    }
}
