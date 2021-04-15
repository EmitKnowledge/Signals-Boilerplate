using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Users;
using App.Domain.Processes.Users.Specification;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Cryptography;
using Signals.Core.Processes.Distributed;
using Signals.Core.Processing.Authentication;
using Signals.Core.Processing.Authorization;
using Signals.Core.Processing.Behaviour;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;

namespace App.Domain.Processes.Users
{
    [Critical]
    [SignalsAuthenticate]
    [SignalsAuthorize(UserType.SystemAdmin, UserType.CompanyAdmin)]
    public class Register : DistributedProcess<RegisterTranisentData, User, MethodResult<User>>
    {
        [Import] private IUserRepository UserRepository { get; set; }

        public override MethodResult<User> Auth(User user)
        {
            return Ok();
        }

        public override MethodResult<User> Validate(User user)
        {
            // override the company id to prevent updates on other companies
            var currentUser = Context.Authentication.GetCurrentUser<User>();
            if (currentUser.Type == UserType.CompanyAdmin)
            {
                user.CompanyId = currentUser.CompanyId;
            }

            return BeginValidation()
                .Validate(new UserNameMatchSpecification(), user)
                .Validate(new UserDataSpecification(), user)
                .Validate(new IsExistingEmailSpecification(), user.Email)
	            .Validate(new IsUniqueUserSpecification((x) => !UserRepository.IsExistingUser(x.Username, x.Email)), user)
                .ReturnResult();
        }

        public override MethodResult<User> Handle(User user)
        {
            var salt = Hashing.GenerateSalt(18);
            user.PasswordSalt = salt;
            user.Password = Hashing.ToSha256(user.Password, salt);
            user.Username = Hashing.GenerateSalt(10);
            user.Id = UserRepository.Insert(user);
            
            return new MethodResult<User>(user);
        }

        /// <summary>
        /// Work process
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override VoidResult Work(RegisterTranisentData request)
        {
            // SEND WELCOME EMAIL TO USER
            return Ok();
        }

        /// <summary>
        /// Map data
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public override RegisterTranisentData Map(User user, MethodResult<User> response)
        {
            var transientData = new RegisterTranisentData()
            {
                Email = user.Email
            };

            return transientData;
        }
    }

    /// <summary>
    /// Transient data that will be passed from the caller to the callee(Handle -> Work)
    /// </summary>
    public class RegisterTranisentData : ITransientData
    {
        public string Email { get; set; }
    }
}
