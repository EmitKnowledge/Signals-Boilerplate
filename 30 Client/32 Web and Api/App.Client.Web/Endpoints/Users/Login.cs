using System.Security.Claims;
using App.Client.Web.Endpoints.Users.Dtos;
using App.Domain.Processes.Generic.Specification;
using NodaTime;
using Signals.Aspects.Auth;
using Signals.Core.Processes;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;

namespace App.Client.Web.Endpoints.Users
{
	/// <summary>
	/// Login user - /api/endpoints/users/login
	/// </summary>
	[SignalsApi(HttpMethod = SignalsApiMethod.POST)]
	public class Login : ApiProcess<LoginUserDto, MethodResult<UserDto>>
    {
	    public override MethodResult<UserDto> Auth(LoginUserDto dto)
        {
	        if (Context.Authentication.GetCurrentPrincipal()?.Identity?.IsAuthenticated == true)
	        {
		        Context.Authentication.Logout();
			}
	        return Ok();
		}

        public override MethodResult<UserDto> Validate(LoginUserDto dto)
        {
	        return BeginValidation()
		        .Validate(new NotNullEntity<LoginUserDto>(), dto)
		        .ReturnResult();
		}

        public override MethodResult<UserDto> Handle(LoginUserDto dto)
        {
	        // get user details for the provided input
	        var loginResult = Continue<Domain.Processes.Users.Login>().With(dto.Email, dto.Password);
	        if (loginResult.IsFaulted) return Fail(loginResult);

	        var user = loginResult.Result;
	        var cookieExpirationDate = SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc();

			// set the cookie
			Context.Authentication.Login(new ClaimsPrincipal(new ClaimsIdentity("Cookies")), user, new AuthenticationProperties
	        {
		        ExpiresUtc = dto.RememberMe ? cookieExpirationDate.AddYears(1) : cookieExpirationDate.AddDays(14),
		        IsPersistent = true,
				AllowRefresh = true
			});
	        Context.Authorization.AddRoles(user.Type.ToString());

	        // Return public information
	        return new MethodResult<UserDto>(user);
		}
    }
}