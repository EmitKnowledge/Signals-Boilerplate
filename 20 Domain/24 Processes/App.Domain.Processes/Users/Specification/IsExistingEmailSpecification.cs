using App.Domain.DataRepositoryContracts;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Check if email exists in database
    /// </summary>
    public class IsExistingEmailSpecification : BaseSpecification<string>
    {
        [Import]
        private IUserRepository UserRepository { get; set; }

        public override bool Validate(string email)
        {
            return !UserRepository.IsExistingUser(null, email);
        }
    }
}
