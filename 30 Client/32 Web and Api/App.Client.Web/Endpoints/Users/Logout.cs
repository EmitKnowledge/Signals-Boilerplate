using Signals.Core.Processes.Api;
using Signals.Core.Processing.Authentication;
using Signals.Core.Processing.Results;

namespace App.Client.Web.Endpoints.Users
{
	/// <summary>
	/// Logout the current user - /api/endpoints/users/logout
	/// </summary>
	[SignalsAuthenticate]
	[SignalsApi(HttpMethod = SignalsApiMethod.GET)]
	public class Logout : ApiProcess<VoidResult>
	{
		public override VoidResult Auth()
		{
			return Ok();
		}

		public override VoidResult Validate()
		{
			return Ok();
		}
		
		public override VoidResult Handle()
		{
			// else, regular logout
			Context.Authentication.Logout();
			return Ok();
		}
	}
}
