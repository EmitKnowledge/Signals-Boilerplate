using App.Domain.Entities.Base;
using System;
using System.Runtime.Serialization;

namespace App.Domain.Entities.Companies
{
    /// <summary>
    /// Represent the company associated with the users
    /// </summary>
    [Serializable]
    [DataContract]
    public class Company : BaseDomainEntity
	{
		/// <summary>
		/// Name of the company
		/// </summary>
		[DataMember]
		public string Name { get; set; }
	}
}