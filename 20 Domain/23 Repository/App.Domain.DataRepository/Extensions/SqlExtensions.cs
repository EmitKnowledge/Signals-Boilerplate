using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using App.Domain.Entities.Common;

namespace App.Domain.DataRepository.Extensions
{
	public static class SqlExtensions
	{
		public static string SqlTrue => "1 = 1";
		public static string SqlFalse => "1 <> 1";
		public static string DefaultOrderBy => "1";

		public static string ToSqlWhereClause(this List<string> filters, SqlOperator op)
		{
			return filters.Count == 0
				? (op == SqlOperator.And ? SqlTrue : SqlFalse)
				: string.Join($" {op.ToString()} ", filters);
		}

		public static string ToSqlOrderByClause(this List<string> sortings, Func<string> @default = null)
		{
			var nonEmptySortings = sortings
				.Where(x => !string.IsNullOrEmpty(x.Trim()) && x.Trim().ToLower() != "asc" && x.Trim().ToLower() != "desc")
				.ToList();

			return nonEmptySortings.Count == 0
				? (@default == null ? DefaultOrderBy : @default())
				: string.Join(", ", nonEmptySortings);
		}

		public static string ToSqlPagedClause(this QueryOptions queryOptions, ExpandoObject executionData)
		{
			var sql =
				@"
                    OFFSET @PageSize * (@Page - 1) ROWS
                    FETCH NEXT @PageSize ROWS ONLY
                ";
			(executionData as IDictionary<string, object>).Add("Page", queryOptions.Page);
			(executionData as IDictionary<string, object>).Add("PageSize", queryOptions.PageSize);

			return sql;
		}
	}

	public enum SqlOperator
	{
		And,
		Or
	}
}
