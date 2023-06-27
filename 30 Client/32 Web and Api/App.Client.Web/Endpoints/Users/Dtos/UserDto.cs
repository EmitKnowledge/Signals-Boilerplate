using System;
using System.Runtime.Serialization;
using App.Client.Web.Endpoints.Companies.Dtos;
using App.Domain.Entities.Companies;
using App.Domain.Entities.Users;
using Ganss.Xss;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Input;

namespace App.Client.Web.Endpoints.Users.Dtos
{
	[DataContract, Serializable]
	public class UserDto : IDtoData
	{
		/// <summary>
		/// Represents the user's id
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Represents the company this user belongs to
		/// </summary>
		[DataMember]
		public CompanyDto Company { get; set; }

		/// <summary>
		/// User's email
		/// </summary>
		[DataMember]
		public string Email { get; set; }

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

		/// <summary>
		/// Indicate if the user has been verified in the system
		/// </summary>
		[DataMember]
		public bool IsVerified { get; set; }

		public void Sanitize(HtmlSanitizer sanitizer)
		{
			Name = sanitizer.Sanitize(Name);
		}

		public static implicit operator User(UserDto u)
		{
			if (u == null) return null;
			return new User
			{
				Id = u.Id,
				Company = u.Company,
				Email = u.Email,
				Name = u.Name,
				Type = u.Type,
				IsVerified = u.IsVerified
			};
		}

		public static implicit operator UserDto(User u)
		{
			if (u == null) return null;
			return new UserDto
			{
				Id = u.Id,
				Company = u.Company.IsNull() ? new Company() { Id = u.CompanyId } : u.Company,
				Email = u.Email,
				Name = u.Name,
				Type = u.Type,
				IsVerified = u.IsVerified
			};
		}
	}
}
