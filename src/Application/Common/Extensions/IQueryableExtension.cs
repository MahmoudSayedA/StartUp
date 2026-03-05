using Application.Common.Abstractions.Collections;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Application.Common.Extensions;

public static class IQueryableExtension
{
    public static IQueryable<T> ApplyFilters<T>(
        this IQueryable<T> query,
        List<FilterDescriptor>? filters,
        HashSet<string> allowedKeys)
    {
        if (filters == null || filters.Count == 0)
            return query;

        //check filters if allowed
        CollectionRequestValidator.ValidateFilters(filters, allowedKeys);

        foreach (var filter in filters)
        {

            query = filter.Operator?.ToLower() switch
            {
                "contains" => query.Where($"{filter.Key}.Contains(@0)", filter.Value),
                "startswith" => query.Where($"{filter.Key}.StartsWith(@0)", filter.Value),
                "in" => ApplyInFilter(query, filter),
                "between" => ApplyBetweenFilter(query, filter),
                "==" => query.Where($"{filter.Key} == @0", ParseValue<T>(filter.Key, filter.Value)),
                "!=" => query.Where($"{filter.Key} != @0", ParseValue<T>(filter.Key, filter.Value)),
                ">" => query.Where($"{filter.Key} > @0", ParseValue<T>(filter.Key, filter.Value)),
                ">=" => query.Where($"{filter.Key} >= @0", ParseValue<T>(filter.Key, filter.Value)),
                "<" => query.Where($"{filter.Key} < @0", ParseValue<T>(filter.Key, filter.Value)),
                "<=" => query.Where($"{filter.Key} <= @0", ParseValue<T>(filter.Key, filter.Value)),
                _ => throw new ValidationException($"Operator '{filter.Operator}' is not supported.")
            };
        }

        return query;
    }

    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        string? sortBy,
        string? sortDirection,
        HashSet<string>? allowedSortFields,
        string defaultSort = "Id")
    {
        //validate
        CollectionRequestValidator.ValidateSorting(sortBy, allowedSortFields);

        if (string.IsNullOrWhiteSpace(sortBy))
        {
            //validate entity has Id
            var prop = typeof(T).GetProperty(defaultSort);
            if (prop != null)
                return query.OrderBy(defaultSort); // Dynamic.Core OrderBy

            else return query;
        }

        var direction = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase)
            ? "descending"
            : "ascending";

        return query.OrderBy($"{sortBy} {direction}");
    }

    public static IQueryable<T> ApplySearch<T>(
    this IQueryable<T> query,
    string? search,
    List<string>? searchColumns,
    HashSet<string> allowedSearchColumns)
    {
        // Nothing to search
        if (string.IsNullOrWhiteSpace(search) || 
            searchColumns == null || 
            searchColumns.Count == 0 || 
            allowedSearchColumns == null || 
            allowedSearchColumns.Count == 0)
            return query;

        //validate search columns
        CollectionRequestValidator.ValidateSearch(searchColumns, allowedSearchColumns);

        // Build: col1.Contains(@0) || col2.Contains(@0) || ...
        var predicate = string.Join(" || ", searchColumns.Select(c => $"{c}.Contains(@0)"));

        return query.Where(predicate, search.Trim());
    }

    // --- Helpers ---

    private static IQueryable<T> ApplyInFilter<T>(IQueryable<T> query, FilterDescriptor filter)
    {
        var rawValues = filter.Value?.Split(',').Select(v => v.Trim()).ToArray();
        if (rawValues == null || rawValues.Length == 0) return query;

        // Parse each value to the actual property type
        var typedValues = rawValues
            .Select(v => ParseValue<T>(filter.Key, v))
            .ToArray();

        // Id == @0 || Id == @1 || Id == @2
        var predicate = string.Join(" || ", typedValues
            .Select((_, i) => $"{filter.Key} == @{i}"));

        return query.Where(predicate, typedValues);
    }

    private static IQueryable<T> ApplyBetweenFilter<T>(IQueryable<T> query, FilterDescriptor filter)
    {
        if (filter.Operator == "between" && (string.IsNullOrWhiteSpace(filter.Value) || string.IsNullOrWhiteSpace(filter.ValueTo)))
        {
            throw new ValidationException("Operator (between) require [value, valueTo] to contains values");
        }

        var from = ParseValue<T>(filter.Key, filter.Value);
        var to = ParseValue<T>(filter.Key, filter.ValueTo);

        if (Comparer<object>.Default.Compare(from, to) > 0)
            throw new ValidationException($"'value' must be less than or equal to 'valueTo' for between operator.");

        return query.Where($"{filter.Key} >= @0 && {filter.Key} <= @1", from, to);
    }

    private static object? ParseValue<T>(string key, string? value)
    {
        if (value == null) return null;

        try
        {
            var prop = typeof(T).GetProperty(key,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (prop == null) return value;

            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            // Handle enums, dates, guids, numerics automatically
            if (targetType == typeof(Guid)) return Guid.Parse(value);
            if (targetType == typeof(DateTimeOffset)) return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture).ToUniversalTime();
            if (targetType == typeof(DateTime)) return DateTime.Parse(value, CultureInfo.InvariantCulture).ToUniversalTime();
            if (targetType == typeof(TimeSpan)) return TimeSpan.Parse(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(bool)) return bool.Parse(value);
            if (targetType.IsEnum) return Enum.Parse(targetType, value, ignoreCase: true);

            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
        {
            throw new ValidationException($"Value '{value}' is not valid for field '{key}'.");
        }

    }

}

public static class CollectionRequestValidator
{
    public static void ValidateFilters(List<FilterDescriptor> filters, HashSet<string>? allowed)
    {
        if (allowed == null || allowed.Count == 0)
            return;

        if (filters != null && filters.Count != 0)
        {
            foreach (var filter in filters)
            {
                if (!allowed.Contains(filter.Key, StringComparer.OrdinalIgnoreCase))
                {
                    List<ValidationFailure> validationFailures = [new ValidationFailure(filter.Key, ValidationErrorMessage(filter.Key, allowed))];
                    throw new ValidationException(validationFailures);
                }
            }
        }
    }

    public static void ValidateSorting(string? sortBy, HashSet<string>? allowed)
    {
        if (allowed == null || allowed.Count == 0)
            return;

        if (!string.IsNullOrEmpty(sortBy) && !allowed.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
        {
            ICollection<ValidationFailure> validationFailures = [new ValidationFailure(sortBy, ValidationErrorMessage(sortBy, allowed))];
            throw new ValidationException(validationFailures);
        }
    }

    public static void ValidateSearch(List<string>? searchColumns, HashSet<string>? allowedSearchColumns)
    {
        if (allowedSearchColumns == null || allowedSearchColumns.Count == 0)
            return;

        if (searchColumns != null && searchColumns.Count != 0)
        {
            foreach (var column in searchColumns)
            {
                if (!string.IsNullOrWhiteSpace(column) && !allowedSearchColumns.Contains(column, StringComparer.OrdinalIgnoreCase))
                {
                    List<ValidationFailure> validationFailures = [new ValidationFailure(column, ValidationErrorMessage(column, allowedSearchColumns))];
                    throw new ValidationException(validationFailures);
                }
            }
        }
    }

    private static string ValidationErrorMessage(string propertyName, IEnumerable<string> allowed)
    {
        return $"Invalid property name ({propertyName}). Only values [{string.Join(", ", allowed)}] are allowed.";
    }
}