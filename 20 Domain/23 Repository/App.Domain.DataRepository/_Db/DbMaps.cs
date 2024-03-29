﻿using App.Domain.Entities.Users;
using DapperExtensions.Mapper;

namespace App.Domain.DataRepository._Db
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