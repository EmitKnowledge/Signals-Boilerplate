using App.Client.Web.Endpoints.Users.Dtos;
using App.Domain.Entities.Users;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Authentication;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Results;

namespace App.Client.Web.Endpoints.Users
{
	[SignalsAuthenticate]
	[SignalsApi(HttpMethod = SignalsApiMethod.GET)]
	public class Me : ApiProcess<MethodResult<UserDto>>
    {
        public override MethodResult<UserDto> Auth()
        {
	        if (Context.Authentication.GetCurrentPrincipal()?.Identity?.IsAuthenticated == false)
	        {
		        return Fail(new AuthenticationErrorInfo());
			}

	        return Ok();
		}

        public override MethodResult<UserDto> Validate()
        {
            return Ok();
        }

        public override MethodResult<UserDto> Handle()
        {
	        var user = Context.Authentication.GetCurrentUser<User>();
	        return new MethodResult<UserDto>(user);
        }
    }
}