using System.Linq;
using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Common;
using App.Domain.Entities.Users;
using App.Domain.Processes.Generic.Specification;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Authentication;
using Signals.Core.Processing.Authorization;
using Signals.Core.Processing.Results;

namespace App.Domain.Processes.Users
{
	[SignalsAuthenticate]
	[SignalsAuthorize(UserType.SystemAdmin, UserType.CompanyAdmin)]
	public class List : BusinessProcess<QueryOptions, ListResult<User>>
    {
	    [Import] private IUserRepository UserRepository { get; set; }

		public override ListResult<User> Auth(QueryOptions queryOptions)
        {
            return Ok();
        }

        public override ListResult<User> Validate(QueryOptions queryOptions)
        {
			return BeginValidation()
		        .Validate(new NotNullEntity<QueryOptions>(), queryOptions)
		        .ReturnResult();
        }

        public override ListResult<User> Handle(QueryOptions queryOptions)
        {
			queryOptions.OverridePagingValuesIfNotValid();

			// get a list of existing users
			var users = UserRepository.Search(queryOptions);
	        var total = UserRepository.SearchCount(queryOptions);
			var result = new ListResult<User>(users);
			result.TotalCount = total;

			return result;
        }
    }
}