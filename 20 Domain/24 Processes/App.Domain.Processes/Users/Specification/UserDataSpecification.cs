using App.Domain.Configuration;
using App.Domain.Entities.Users;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Regexes;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Check if the provided user data is valid
    /// </summary>
    public class UserDataSpecification : BaseSpecification<User>
    {
        #region Overrides of BaseSpecification<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(User x)
        {
            return
                x.CompanyId > 0 &&
                (x.Id > 0 || (x.Id == 0 && !x.Password.IsNullOrEmpty() && x.Password.Length <= 20)) &&
                !x.Email.IsNullOrEmpty() && x.Email.Length <= 120 &&
                x.Email.IsMatch(@"^([0-9a-zA-Z]([+-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        }

        #endregion Overrides of BaseSpecification<User>
    }
}
