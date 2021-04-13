using App.Domain.Entities.Common;
using App.Domain.Entities.Users;
using System.Collections.Generic;

namespace App.Domain.DataRepositoryContracts
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Get user with password and password hash included
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        User GetUserByEmailWithCriticalDataIncluded(string email);

        /// <summary>
        /// Search against the users
        /// No filters will return all data order by name asc
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        List<User> Search(QueryOptions queryOptions);

        /// <summary>
        /// Return total count for the provided query options
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        int SearchCount(QueryOptions queryOptions);

        /// <summary>
        /// Check if an user is exiting (check is done by both username and email)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        bool IsExistingUser(string username, string email);
    }
}