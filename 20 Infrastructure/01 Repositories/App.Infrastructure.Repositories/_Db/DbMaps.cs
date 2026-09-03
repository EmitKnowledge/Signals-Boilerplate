using App.Core.Domain.Users;
using DapperExtensions.Mapper;

namespace App.Infrastructure.Repositories._Db
{
    /// <summary>
    /// Mapping for user table
    /// </summary>
    public class UserMapper : ClassMapper<User>
    {
        public UserMapper()
        {
            Table("User");
            Map(x => x.Company).Ignore();
            Map(x => x.CompanyId).Ignore();
            AutoMap();
        }
    }
}