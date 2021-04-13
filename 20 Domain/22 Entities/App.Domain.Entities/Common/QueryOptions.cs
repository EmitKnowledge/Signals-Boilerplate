using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace App.Domain.Entities.Common
{
	/// <summary>
	/// Represents querying options for the entity
	/// </summary>
	public class QueryOptions
	{
		/// <summary>
		/// Represents search word
		/// </summary>
		public string SearchTerm { get; set; }

		/// <summary>
		/// Represents the page number
		/// </summary>
		public int Page { get; set; }

		/// <summary>
		/// Represents the number of entities returned by the query processor
		/// </summary>
		public int PageSize { get; set; }

		/// <summary>
		/// Represents the ascending or descending ordering
		/// </summary>
		public List<Order> Orders { get; set; }

		/// <summary>
		/// Represents collection of filters
		/// </summary>
		public List<Filter> Filters { get; set; }

		public void Normalize()
		{
			Orders = Orders.Where(x => !string.IsNullOrEmpty(x.Field.Trim())).ToList();
			Filters = Filters.Where(x => !string.IsNullOrEmpty(x.Name.Trim())).ToList();
		}

		/// <summary>
		/// Check if pagination values are valid and override with provided values if not
		/// </summary>
		public void OverridePagingValuesIfNotValid(int page = 1, int pageSize = 10)
		{
			if (Page < 1)
			{
				Page = page;
			}
			if (PageSize < 1)
			{
				PageSize = pageSize;
			}
		}

		/// <summary>
		/// Add a filter and overrides the system if it exists
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void AddFilter(string name, string value)
		{
			Filters.RemoveAll(x => string.Compare(x.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0);
			Filters.Add(new Filter
			{
				Name = name,
				Value = value
			});
		}

		/// <summary>
		/// Ctor with default values
		/// </summary>
		public QueryOptions()
		{
			Page = 1;
			PageSize = 10;
			Filters = new List<Filter>();
			Orders = new List<Order>();
		}
	}

	/// <summary>
	/// Represents filter model
	/// </summary>
	public class Filter
	{
		/// <summary>
		/// Represents the entity's field being filtered by
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Represents the filter value
		/// </summary>
		public string Value { get; set; }
	}

	/// <summary>
	/// Represents order model
	/// </summary>
	public class Order
	{
		/// <summary>
		/// Represents the entity's field being ordered by
		/// </summary>
		public string Field { get; set; }

		/// <summary>
		/// Represents the order type
		/// </summary>
		public OrderType OrderType { get; set; }
	}

	/// <summary>
	/// Represents the ordering type
	/// </summary>
	public enum OrderType
	{
		/// <summary>
		/// Ascending order
		/// </summary>
		[EnumMember]
		Asc,

		/// <summary>
		/// Descending order
		/// </summary>
		[EnumMember]
		Desc
	}
}