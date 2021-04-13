using Ganss.XSS;
using Signals.Core.Processing.Input;

namespace App.Client.Web.Endpoints.Base.Dtos
{
	public class ByDtoIdDto : IDtoData
	{
		/// <summary>
		/// Represents the ID of the entity
		/// </summary>
		public int Id { get; set; }

		public void Sanitize(HtmlSanitizer sanitizer)
		{
		}
	}
}
