using System.Runtime.Serialization;

namespace App.Core.Domain.Base
{
    public interface IBaseDomainEntity<TKey>
    {
        /// <summary>
        /// Entity Id
        /// </summary>
        [DataMember]
        int Id { get; set; }
    }
}