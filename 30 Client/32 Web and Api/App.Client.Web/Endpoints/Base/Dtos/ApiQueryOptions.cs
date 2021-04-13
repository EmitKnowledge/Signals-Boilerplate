using App.Domain.Entities.Common;
using Ganss.XSS;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Input;

namespace App.Client.Web.Endpoints.Base.Dtos
{
	public class ApiQueryOptions : QueryOptions, IDtoData
	{
		public void Sanitize(HtmlSanitizer sanitizer)
		{
			if (!SearchTerm.IsNullOrEmpty())
			{
				SearchTerm = sanitizer.Sanitize(SearchTerm);
			}

			foreach (var filter in Filters)
			{
				if (filter.IsNull()) continue;
				if (!filter.Name.IsNullOrEmpty())
				{
					filter.Name = sanitizer.Sanitize(filter.Name);
				}
				if (!filter.Value.IsNullOrEmpty())
				{
					filter.Value = sanitizer.Sanitize(filter.Value);
				}
			}

			foreach (var order in Orders)
			{
				if (order.IsNull()) continue;
				if (!order.Field.IsNullOrEmpty())
				{
					order.Field = sanitizer.Sanitize(order.Field);
				}
			}
		}
	}
}
