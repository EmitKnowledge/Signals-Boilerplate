using App.Domain.Entities.Users;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Regexes;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Check if the First, Last and Middle Name are valid
    /// </summary>
    public class UserNameMatchSpecification : BaseSpecification<User>
    {
        #region Overrides of BaseSpecification<User>
        
        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override bool Validate(User x)
        {
            return
                !x.Name.IsNullOrEmpty() && x.Name.Length <= 30 &&
                !x.Name.IsMatch(@"[0-9]");
        }

        #endregion Overrides of BaseSpecification<User>
    }
}