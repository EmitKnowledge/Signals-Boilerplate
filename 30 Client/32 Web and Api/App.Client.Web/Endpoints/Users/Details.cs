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
	/// Returns details for the provided user id - /api/endpoints/users/details?id=1
	/// </summary>
	[SignalsAuthenticate]
	[SignalsAuthorize(UserType.SystemAdmin, UserType.CompanyAdmin)]
	[SignalsApi(HttpMethod = SignalsApiMethod.GET)]
	public class Details : ApiProcess<ByDtoIdDto, MethodResult<UserDto>>
    {
        public override MethodResult<UserDto> Auth(ByDtoIdDto dto)
        {
	        return Ok();
		}

        public override MethodResult<UserDto> Validate(ByDtoIdDto dto)
        {
	        return BeginValidation()
		        .Validate(new NotNullEntity<ByDtoIdDto>(), dto)
		        .ReturnResult();
		}

        public override MethodResult<UserDto> Handle(ByDtoIdDto dto)
        {
	        var result = Continue<Domain.Processes.Users.Details>().With(dto.Id);
	        if (result.IsFaulted) return Fail(result);
	        return new MethodResult<UserDto>(result.Result);
		}
    }
}