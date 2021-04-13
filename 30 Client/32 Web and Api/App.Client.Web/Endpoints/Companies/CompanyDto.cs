using System;
using System.Runtime.Serialization;
using App.Domain.Entities.Companies;
using Ganss.XSS;
using Signals.Core.Processing.Input;

namespace App.Client.Web.Endpoints.Companies.Dtos
{
    /// <summary>
    /// Represent a company DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class CompanyDto : IDtoData
	{
		/// <summary>
		/// Id of the company
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Name of the company
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		public void Sanitize(HtmlSanitizer sanitizer)
		{
			Name = sanitizer.Sanitize(Name);
		}

		public static implicit operator Company(CompanyDto c)
		{
			if (c == null) return null;
			return new Company
			{
				Id = c.Id,
				Name = c.Name
			};
		}

		public static implicit operator CompanyDto(Company c)
		{
			if (c == null) return null;
			return new CompanyDto
			{
				Id = c.Id,
				Name = c.Name
			};
		}
	}
}