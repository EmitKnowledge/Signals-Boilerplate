using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Users;
using App.Domain.Processes.Generic.Specification;
using Signals.Aspects.DI.Attributes;
using Signals.Aspects.Localization;
using Signals.Core.Common.Cryptography;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Results;

namespace App.Domain.Processes.Users
{
	public class Login : BusinessProcess<string, string, MethodResult<User>>
    {
	    [Import]
	    private ILocalizationProvider LocalizationProvider { get; set; }

		[Import]
		private IUserRepository UserRepository { get; set; }

		public override MethodResult<User> Auth(string email, string password)
        {
	        return Ok();
		}

        public override MethodResult<User> Validate(string email, string password)
        {
	        return BeginValidation()
	            .Validate(new NotNullOrEmptyString(), email)
	            .Validate(new NotNullOrEmptyString(), password)
	            .ReturnResult();
		}

        public override MethodResult<User> Handle(string email, string password)
        {
			var user = UserRepository.GetUserByEmailWithCriticalDataIncluded(email);
			
			if (user == null)
			{
				// use localization provider to get content for the provided KEY
				return Fail(new GeneralErrorInfo("Invalid credentials", LocalizationProvider.Get("INVALID_CREDENTIALS")?.Value ?? ""));
			}
			
			if (!Hashing.VerifySha256(user.Password, password, user.PasswordSalt))
			{
				return Fail(new GeneralErrorInfo("Invalid credentials", LocalizationProvider.Get("INVALID_CREDENTIALS")?.Value ?? ""));
			}

			return user;
		}
    }
}