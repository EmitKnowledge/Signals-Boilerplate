using App.Domain.DataRepository.Base;
using App.Domain.DataRepository.Extensions;
using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Common;
using App.Domain.Entities.Users;
using Dapper;
using Signals.Aspects.DI.Attributes;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace App.Domain.DataRepository.Users
{
    [Export(typeof(IUserRepository))]
    internal class UserRepository : BaseDbRepository<User>, IUserRepository
    {
        /// <summary>
        /// Get user with password and password hash included
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmailWithCriticalDataIncluded(string email)
        {
            return base.FirstOrDefault(Projection<User>.Default, x => x.Email == email);
        }

        /// <summary>
        /// Return user by id, allow selection of which properties will be retrieved
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override User GetById(int id)
        {
            var existingUser = base.FirstOrDefault(Projection<User>.Default, user => user.Id == id);
            if (existingUser != null)
            {
                existingUser.Password = null;
                existingUser.PasswordSalt = null;
            }
            return existingUser;
        }

        /// <summary>
        /// Search against the users
        /// No filters will return all data order by name asc
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        public List<User> Search(QueryOptions queryOptions)
        {
            return Using(connection =>
            {
                dynamic executionData = new ExpandoObject();
                // filters
                var whereClause = queryOptions.Filters.Select(x =>
                {
                    switch (x.Name)
                    {
                        case "CompanyId":
                            int.TryParse(x.Value, out var companyId);
                            executionData.CompanyId = companyId;
                            return "u.CompanyId = @CompanyId";
                        case "Type":
                            int.TryParse(x.Value, out var type);
                            executionData.Type = type;
                            return "u.Type = @Type";
                        default:
                            return SqlExtensions.SqlTrue;
                    }
                }).ToList().ToSqlWhereClause(SqlOperator.And);

                // orders
                var orderByClause = queryOptions.Orders.Select(x =>
                {
                    string orderBy;
                    switch (x.Field)
                    {
                        case "Name":
                            orderBy = "u.Name";
                            break;
                        default:
                            orderBy = "u.Name";
                            break;
                    }
                    return $"{orderBy} {x.OrderType}";
                }).ToList().ToSqlOrderByClause(() => "u.Name ASC");

                var pagingClause = queryOptions.ToSqlPagedClause(executionData as ExpandoObject);

                var sql =
                    $@"
                        SELECT *
	                    FROM [User] u
	                    WHERE {whereClause}
                        ORDER BY {orderByClause}
                        {pagingClause}
                    ";

                var result = connection.Query<User>(sql, (object)executionData);
                return result.ToList();
            });
        }

        /// <summary>
        /// Return total count for the provided query options
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        public int SearchCount(QueryOptions queryOptions)
        {
            return Using(connection =>
            {
                dynamic executionData = new ExpandoObject();
                // filters
                var whereClause = queryOptions.Filters.Select(x =>
                {
                    switch (x.Name)
                    {
                        case "CompanyId":
                            int.TryParse(x.Value, out var companyId);
                            executionData.CompanyId = companyId;
                            return "u.CompanyId = @CompanyId";
                        case "Type":
                            int.TryParse(x.Value, out var type);
                            executionData.Type = type;
                            return "u.Type = @Type";
                        default:
                            return SqlExtensions.SqlTrue;
                    }
                }).ToList().ToSqlWhereClause(SqlOperator.And);

                var pagingClause = queryOptions.ToSqlPagedClause(executionData as ExpandoObject);

                var sql =
                    $@"
                        SELECT COUNT(u.Id)
	                    FROM [User] u
	                    WHERE {whereClause}
                    ";

                var result = connection.ExecuteScalar<int>(sql, (object)executionData);
                return result;
            });
        }

        /// <summary>
        /// Check if an user is exiting (check is done by both username and email)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsExistingUser(string username, string email)
        {
            User existingUser = null;
            if (!string.IsNullOrEmpty(username))
            {
                existingUser = base.FirstOrDefault(Projection<User>.Default, x => x.Username == username);
            }
            else if (!string.IsNullOrEmpty(email))
            {
                existingUser = base.FirstOrDefault(Projection<User>.Default, x => x.Email == email);
            }
            return existingUser != null;
        }
    }
}