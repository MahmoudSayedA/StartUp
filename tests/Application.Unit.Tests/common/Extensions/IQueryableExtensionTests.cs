using Application.Common.Abstractions.Collections;
using Application.Common.Extensions;
using Application.Common.Models;

namespace Application.Unit.Tests.common.Extensions;
public class QueryableExtensionTests
{
    public class TestEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void ApplyFilters_ShouldFilterQueryable_WhenFiltersAreProvided()
    {
        // Arrange
        var data = new List<TestEntity>
        {
            new TestEntity { Name = "Alice" },
            new TestEntity { Name = "Bob" },
            new TestEntity { Name = "Charlie" }
        }.AsQueryable();

        var filters = new List<FilterDescriptor>
        {
            new()
            {
                Key = "Name",
                Operator = "startsWith",
                Value = "A",
            }
        };
        var allowedColumns = new HashSet<string>() { "Name" };

        // Act
        var list = data.ApplyFilters(filters, allowedColumns).ToList();
        
        // Assert
        Assert.Single(list);
        Assert.Equal("Alice", list[0].Name);
    }

}

/*
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
           if (filter.Operator == "between" && (string.IsNullOrWhiteSpace(filter.Value) || string.IsNullOrWhiteSpace(filter.ValueTo)))
           {
               throw new ValidationException("Operator (between) require [value, valueTo] to contains values");
           }
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

        */