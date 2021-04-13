using System.Runtime.Serialization;

namespace App.Domain.Entities.Base
{
    public interface IBaseCompanyDomainEntity : IBaseDomainEntity<int>
	{
        /// <summary>
        /// Represents the company id to which this entity belongs to
        /// </summary>
        [DataMember]
        int CompanyId { get; set; }
    }
}