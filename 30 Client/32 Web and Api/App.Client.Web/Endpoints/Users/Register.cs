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
    [SignalsAuthenticate]
    [SignalsAuthorize(UserType.SystemAdmin, UserType.CompanyAdmin)]
    [SignalsApi(HttpMethod = SignalsApiMethod.POST)]
    public class Register : ApiProcess<RegisterUserDto, MethodResult<UserDto>>
    {
        public override MethodResult<UserDto> Auth(RegisterUserDto dto)
        {
            return Ok();
        }

        public override MethodResult<UserDto> Validate(RegisterUserDto dto)
        {
            return BeginValidation()
                .Validate(new NotNullEntity<RegisterUserDto>(), dto)
                .ReturnResult();
        }

        public override MethodResult<UserDto> Handle(RegisterUserDto dto)
        {
            User user = dto;
            var result = Continue<Domain.Processes.Users.Register>().With(user);
            if (result.IsFaulted) return Fail(result);
            return new MethodResult<UserDto>(result.Result);
        }
    }
}
