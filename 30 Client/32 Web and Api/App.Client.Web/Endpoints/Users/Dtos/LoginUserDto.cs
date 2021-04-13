using Ganss.XSS;
using Signals.Core.Processing.Input;

namespace App.Client.Web.Endpoints.Users.Dtos
{
	public class LoginUserDto : IDtoData
	{
		/// <summary>
		/// Represents the user's email
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Represents the user's password
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Represents the user's re-typed password
		/// </summary>
		public bool RememberMe { get; set; }

		public void Sanitize(HtmlSanitizer sanitizer)
		{
			Email = sanitizer.Sanitize(Email);
			Password = sanitizer.Sanitize(Password);
		}
	}
}
