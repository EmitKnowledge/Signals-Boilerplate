using System;
using App.Domain.Entities.Users;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Check if a user is existing in database
    /// </summary>
    public class IsUniqueUserSpecification : BaseSpecification<User>
    {
        public IsUniqueUserSpecification(Func<User, bool> isUniqueCompanyDelegate)
        {
	        IsUniqueCompanyDelegate = isUniqueCompanyDelegate;
        }

        private Func<User, bool> IsUniqueCompanyDelegate { get; set; }

		#region Overrides of BaseSpecification<User>

		/// <summary>
		/// Validation expression that must be fullfilled
		/// </summary>
		/// <returns></returns>
		public override bool Validate(User x)
        {
            return IsUniqueCompanyDelegate(x);
        }

        #endregion Overrides of BaseSpecification<User>
    }
}