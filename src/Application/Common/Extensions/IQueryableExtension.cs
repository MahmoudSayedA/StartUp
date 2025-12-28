using System.Linq.Dynamic.Core;

namespace Application.Common.Extensions;

public static class IQueryableExtension
{
    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, Dictionary<string, string>? filters)
    {
        if (filters != null && filters.Count != 0)
        {
            foreach (var filter in filters)
            {
                query = query.Where($"{filter.Key}.Contains(@0)", filter.Value);
            }
        }

        return query;
    }

    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, string? sortDirection)
    {
        if (!string.IsNullOrEmpty(sortBy))
        {
            var direction = string.IsNullOrEmpty(sortDirection) || string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase)
                            ? "descending"
                            : "ascending";
            query = query.OrderBy($"{sortBy} {direction}");
        }

        return query;
    }
}
