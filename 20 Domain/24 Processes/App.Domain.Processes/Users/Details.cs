using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Users;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Authentication;
using Signals.Core.Processing.Authorization;
using Signals.Core.Processing.Results;

namespace App.Domain.Processes.Users
{
	[SignalsAuthenticate]
	[SignalsAuthorize(UserType.SystemAdmin, UserType.CompanyAdmin)]
	public class Details : BusinessProcess<int, MethodResult<User>>
    {
	    [Import] private IUserRepository UserRepository { get; set; }

		public override MethodResult<User> Auth(int id)
        {
            return Ok();
        }

        public override MethodResult<User> Validate(int id)
        {
            return Ok();
        }

        public override MethodResult<User> Handle(int id)
        {
			// get current logged in user from Signal process context
	        var user = Context.Authentication.GetCurrentUser<User>();
	        User userDetails = null;
	        if (user.Type == UserType.SystemAdmin)
	        {
				userDetails = UserRepository.GetById(id);
			}
	        else if (user.Type == UserType.CompanyAdmin)
			{
		        userDetails = UserRepository.FirstOrDefault(null, x => x.Id == id && x.CompanyId == user.CompanyId);
			}

			return userDetails;
        }
    }
}