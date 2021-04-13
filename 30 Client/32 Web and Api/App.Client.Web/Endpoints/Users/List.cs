using System.Linq;
using App.Client.Web.Endpoints.Base.Dtos;
using App.Client.Web.Endpoints.Users.Dtos;
using App.Domain.Entities.Users;
using App.Domain.Processes.Generic.Specification;
using Signals.Core.Processes;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Authentication;
using Signals.Core.Processing.Authorization;
using Signals.Core.Processing.Results;

namespace App.Client.Web.Endpoints.Users
{
	/// <summary>
	/// List existing users - /api/endpoints/users/list?queryOptions=JSON.SERIALIZE(queryOptions)
	/// </summary>
	[SignalsAuthenticate]
	[SignalsAuthorize(UserType.SystemAdmin, UserType.CompanyAdmin)]
	[SignalsApi(HttpMethod = SignalsApiMethod.GET)]
	public class List : ApiProcess<ApiQueryOptions, ListResult<UserDto>>
    {
        public override ListResult<UserDto> Auth(ApiQueryOptions queryOptions)
        {
            return Ok();
        }

        public override ListResult<UserDto> Validate(ApiQueryOptions queryOptions)
        {
	        return BeginValidation()
		        .Validate(new NotNullEntity<ApiQueryOptions>(), queryOptions)
		        .ReturnResult();
		}

        public override ListResult<UserDto> Handle(ApiQueryOptions queryOptions)
        {
	        var result = Continue<Domain.Processes.Users.List>().With(queryOptions);
	        if (result.IsFaulted) return Fail(result);

	        var userDtos = result.Result.Select(x => (UserDto)x).ToList();
	        
	        return new ListResult<UserDto>(userDtos)
	        {
				TotalCount = result.TotalCount
	        };
		}
    }
}