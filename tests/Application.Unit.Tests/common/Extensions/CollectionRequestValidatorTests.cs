using Application.Common.Abstractions.Collections;
using Application.Common.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Application.Unit.Tests.common.Extensions;

// ============================================================
//  TEST HELPERS — inline stubs so tests are self-contained
// ============================================================

#region Test Entity & FilterDescriptor stubs

/// <summary>Rich test entity covering all ParseValue branches.</summary>
public class TestEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid ExternalId { get; set; }
    public decimal Price { get; set; }
    public TestStatus Status { get; set; }
    public int? NullableAge { get; set; }
}

public enum TestStatus { Active, Inactive, Pending }

#endregion

// ============================================================
//  COLLECTION REQUEST VALIDATOR TESTS
// ============================================================

public class CollectionRequestValidatorTests
{
    // ── ValidateFilters ──────────────────────────────────────

    [Fact]
    public void ValidateFilters_AllowedIsNull_DoesNotThrow()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Anything" } };
        var ex = Record.Exception(() => CollectionRequestValidator.ValidateFilters(filters, null));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateFilters_AllowedIsEmpty_DoesNotThrow()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Anything" } };
        var ex = Record.Exception(() => CollectionRequestValidator.ValidateFilters(filters, new HashSet<string>()));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateFilters_ValidKey_DoesNotThrow()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Name" } };
        var allowed = new HashSet<string> { "Name", "Email" };
        var ex = Record.Exception(() => CollectionRequestValidator.ValidateFilters(filters, allowed));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateFilters_KeyCaseInsensitive_DoesNotThrow()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "name" } };
        var allowed = new HashSet<string> { "Name" };
        var ex = Record.Exception(() => CollectionRequestValidator.ValidateFilters(filters, allowed));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateFilters_InvalidKey_ThrowsValidationException()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Secret" } };
        var allowed = new HashSet<string> { "Name", "Email" };
        var ex = Assert.Throws<ValidationException>(() =>
            CollectionRequestValidator.ValidateFilters(filters, allowed));
        Assert.Contains("Secret", ex.Message);
    }

    [Fact]
    public void ValidateFilters_MultipleFilters_ThrowsOnFirstInvalid()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { Key = "Name" },
            new() { Key = "HackerField" },
        };
        var allowed = new HashSet<string> { "Name" };
        Assert.Throws<ValidationException>(() =>
            CollectionRequestValidator.ValidateFilters(filters, allowed));
    }

    [Fact]
    public void ValidateFilters_EmptyFiltersList_DoesNotThrow()
    {
        var ex = Record.Exception(() =>
            CollectionRequestValidator.ValidateFilters(new List<FilterDescriptor>(), new HashSet<string> { "Name" }));
        Assert.Null(ex);
    }

    // ── ValidateSorting ──────────────────────────────────────

    [Fact]
    public void ValidateSorting_NullSortBy_DoesNotThrow()
    {
        var ex = Record.Exception(() =>
            CollectionRequestValidator.ValidateSorting(null, new HashSet<string> { "Name" }));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateSorting_EmptySortBy_DoesNotThrow()
    {
        var ex = Record.Exception(() =>
            CollectionRequestValidator.ValidateSorting("", new HashSet<string> { "Name" }));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateSorting_AllowedIsNull_DoesNotThrow()
    {
        var ex = Record.Exception(() =>
            CollectionRequestValidator.ValidateSorting("Name", null));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateSorting_ValidField_DoesNotThrow()
    {
        var ex = Record.Exception(() =>
            CollectionRequestValidator.ValidateSorting("Name", new HashSet<string> { "Name", "Email" }));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateSorting_CaseInsensitive_DoesNotThrow()
    {
        var ex = Record.Exception(() =>
            CollectionRequestValidator.ValidateSorting("name", new HashSet<string> { "Name" }));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateSorting_InvalidField_ThrowsValidationException()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            CollectionRequestValidator.ValidateSorting("HackerField", new HashSet<string> { "Name" }));
        Assert.Contains("HackerField", ex.Message);
    }

    // ── ValidateSearch ───────────────────────────────────────

    [Fact]
    public void ValidateSearch_NullColumns_DoesNotThrow()
    {
        var ex = Record.Exception(() =>
            CollectionRequestValidator.ValidateSearch(null, new HashSet<string> { "Name" }));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateSearch_AllowedIsNull_DoesNotThrow()
    {
        var ex = Record.Exception(() =>
            CollectionRequestValidator.ValidateSearch(new List<string> { "Name" }, null));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateSearch_ValidColumn_DoesNotThrow()
    {
        var ex = Record.Exception(() =>
            CollectionRequestValidator.ValidateSearch(
                new List<string> { "Name" },
                new HashSet<string> { "Name", "Email" }));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateSearch_CaseInsensitive_DoesNotThrow()
    {
        var ex = Record.Exception(() =>
            CollectionRequestValidator.ValidateSearch(
                new List<string> { "name" },
                new HashSet<string> { "Name" }));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateSearch_InvalidColumn_ThrowsValidationException()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            CollectionRequestValidator.ValidateSearch(
                new List<string> { "Secret" },
                new HashSet<string> { "Name" }));
        Assert.Contains("Secret", ex.Message);
    }

    [Fact]
    public void ValidateSearch_ErrorMessage_ContainsAllowedValues()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            CollectionRequestValidator.ValidateSearch(
                new List<string> { "Bad" },
                new HashSet<string> { "Name", "Email" }));
        Assert.Contains("Name", ex.Message);
        Assert.Contains("Email", ex.Message);
    }
}

// ============================================================
//  APPLY FILTERS TESTS  (in-memory LINQ — no EF needed)
// ============================================================

public class ApplyFiltersTests
{
    private static IQueryable<TestEntity> Data() => new List<TestEntity>
    {
        new() { Id = 1, Name = "Rose Garden",    Category = "Flowers", IsActive = true,  Price = 10.5m, Status = TestStatus.Active,   NullableAge = 25 },
        new() { Id = 2, Name = "Rosemary Herb",  Category = "Herbs",   IsActive = true,  Price = 5.0m,  Status = TestStatus.Pending,  NullableAge = null },
        new() { Id = 3, Name = "Blue Orchid",    Category = "Flowers", IsActive = false, Price = 20.0m, Status = TestStatus.Inactive, NullableAge = 30 },
        new() { Id = 4, Name = "Cactus",         Category = "Desert",  IsActive = true,  Price = 8.0m,  Status = TestStatus.Active,   NullableAge = 18 },
        new() { Id = 5, Name = "Jasmine",        Category = "Flowers", IsActive = false, Price = 15.0m, Status = TestStatus.Pending,  NullableAge = 22 },
    }.AsQueryable();

    private static readonly HashSet<string> AllowedFilters =
        ["Id", "Name", "Category", "IsActive", "Price", "Status", "NullableAge"];

    // ── Null / empty guards ──────────────────────────────────

    [Fact]
    public void ApplyFilters_NullFilters_ReturnsOriginalQuery()
    {
        var result = Data().ApplyFilters(null, AllowedFilters).ToList();
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void ApplyFilters_EmptyFilters_ReturnsOriginalQuery()
    {
        var result = Data().ApplyFilters(new List<FilterDescriptor>(), AllowedFilters).ToList();
        Assert.Equal(5, result.Count);
    }

    // ── String operators ─────────────────────────────────────

    [Fact]
    public void ApplyFilters_Contains_ReturnsMatchingRows()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Name", Operator = "contains", Value = "rose" } };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        // "Rose Garden" and "Rosemary Herb" contain "rose" (case-sensitive in LINQ-to-objects)
        Assert.All(result, r => Assert.Contains("rose", r.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ApplyFilters_StartsWith_ReturnsMatchingRows()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Name", Operator = "startswith", Value = "Rose" } };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.StartsWith("Rose", r.Name));
    }

    // ── Equality ─────────────────────────────────────────────

    [Fact]
    public void ApplyFilters_Equals_Bool_ReturnsActiveOnly()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "IsActive", Operator = "==", Value = "true" } };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        Assert.Equal(3, result.Count);
        Assert.All(result, r => Assert.True(r.IsActive));
    }

    [Fact]
    public void ApplyFilters_NotEquals_ReturnsCorrectRows()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Category", Operator = "!=", Value = "Flowers" } };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        Assert.All(result, r => Assert.NotEqual("Flowers", r.Category));
    }

    // ── Numeric comparisons ──────────────────────────────────

    [Fact]
    public void ApplyFilters_GreaterThan_Int_ReturnsCorrectRows()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Id", Operator = ">", Value = "3" } };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.True(r.Id > 3));
    }

    [Fact]
    public void ApplyFilters_GreaterThanOrEqual_Decimal_ReturnsCorrectRows()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Price", Operator = ">=", Value = "10.5" } };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        Assert.All(result, r => Assert.True(r.Price >= 10.5m));
    }

    [Fact]
    public void ApplyFilters_LessThan_ReturnsCorrectRows()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Price", Operator = "<", Value = "10" } };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        Assert.All(result, r => Assert.True(r.Price < 10m));
    }

    [Fact]
    public void ApplyFilters_LessThanOrEqual_ReturnsCorrectRows()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Id", Operator = "<=", Value = "2" } };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        Assert.Equal(2, result.Count);
    }

    // ── Enum ─────────────────────────────────────────────────

    [Fact]
    public void ApplyFilters_Equals_Enum_ReturnsCorrectRows()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Status", Operator = "==", Value = "Active" } };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(TestStatus.Active, r.Status));
    }

    // ── Between ──────────────────────────────────────────────

    [Fact]
    public void ApplyFilters_Between_ReturnsRowsInRange()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { Key = "Price", Operator = "between", Value = "8", ValueTo = "15" }
        };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        Assert.All(result, r => Assert.True(r.Price >= 8m && r.Price <= 15m));
    }

    [Fact]
    public void ApplyFilters_Between_MissingValue_ThrowsValidationException()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { Key = "Price", Operator = "between", Value = "8", ValueTo = null }
        };
        Assert.Throws<ValidationException>(() =>
            Data().ApplyFilters(filters, AllowedFilters).ToList());
    }

    [Fact]
    public void ApplyFilters_Between_MissingValueTo_ThrowsValidationException()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { Key = "Price", Operator = "between", Value = null, ValueTo = "15" }
        };
        Assert.Throws<ValidationException>(() =>
            Data().ApplyFilters(filters, AllowedFilters).ToList());
    }

    // ── In operator ──────────────────────────────────────────

    [Fact]
    public void ApplyFilters_In_ReturnsMatchingRows()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { Key = "Category", Operator = "in", Value = "Flowers,Herbs" }
        };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        Assert.Equal(4, result.Count);
        Assert.All(result, r => Assert.True(r.Category == "Flowers" || r.Category == "Herbs"));
    }

    [Fact]
    public void ApplyFilters_In_EmptyValue_ReturnsOriginalQuery()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { Key = "Category", Operator = "in", Value = null }
        };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        Assert.Equal(5, result.Count);
    }

    // ── Multiple filters (AND logic) ─────────────────────────

    [Fact]
    public void ApplyFilters_MultipleFilters_AppliesAndLogic()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { Key = "Category", Operator = "==",       Value = "Flowers" },
            new() { Key = "IsActive",  Operator = "==",      Value = "true" },
        };
        var result = Data().ApplyFilters(filters, AllowedFilters).ToList();
        Assert.Single(result);
        Assert.Equal("Rose Garden", result[0].Name);
    }

    // ── Security: disallowed key ─────────────────────────────

    [Fact]
    public void ApplyFilters_DisallowedKey_ThrowsValidationException()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Secret", Operator = "==", Value = "x" } };
        Assert.Throws<ValidationException>(() =>
            Data().ApplyFilters(filters, AllowedFilters).ToList());
    }

    // ── Unknown operator ─────────────────────────────────────

    [Fact]
    public void ApplyFilters_UnknownOperator_ThrowsValidationException()
    {
        var filters = new List<FilterDescriptor> { new() { Key = "Name", Operator = "LIKE", Value = "rose" } };
        Assert.Throws<ValidationException>(() =>
            Data().ApplyFilters(filters, AllowedFilters).ToList());
    }
}

// ============================================================
//  APPLY SORTING TESTS
// ============================================================

public class ApplySortingTests
{
    private static IQueryable<TestEntity> Data() => new List<TestEntity>
    {
        new() { Id = 3, Name = "Cactus",   Price = 8.0m  },
        new() { Id = 1, Name = "Rose",     Price = 10.5m },
        new() { Id = 2, Name = "Jasmine",  Price = 5.0m  },
    }.AsQueryable();

    private static readonly HashSet<string> AllowedSort = ["Id", "Name", "Price"];

    [Fact]
    public void ApplySorting_NullSortBy_UsesDefaultSort()
    {
        var result = Data().ApplySorting(null, null, AllowedSort, "Id").ToList();
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
        Assert.Equal(3, result[2].Id);
    }

    [Fact]
    public void ApplySorting_Ascending_SortsCorrectly()
    {
        var result = Data().ApplySorting("Name", "asc", AllowedSort).ToList();
        Assert.Equal("Cactus", result[0].Name);
        Assert.Equal("Jasmine", result[1].Name);
        Assert.Equal("Rose", result[2].Name);
    }

    [Fact]
    public void ApplySorting_Descending_SortsCorrectly()
    {
        var result = Data().ApplySorting("Price", "desc", AllowedSort).ToList();
        Assert.True(result[0].Price >= result[1].Price);
        Assert.True(result[1].Price >= result[2].Price);
    }

    [Fact]
    public void ApplySorting_InvalidSortField_ThrowsValidationException()
    {
        Assert.Throws<ValidationException>(() =>
            Data().ApplySorting("HackerField", "asc", AllowedSort).ToList());
    }

    [Fact]
    public void ApplySorting_CaseInsensitiveDirection_Desc_SortsCorrectly()
    {
        var result = Data().ApplySorting("Id", "DESC", AllowedSort).ToList();
        Assert.Equal(3, result[0].Id);
    }

    [Fact]
    public void ApplySorting_InvalidDirection_DefaultsToAscending()
    {
        // Any non-"desc" string should default to ascending
        var result = Data().ApplySorting("Id", "random", AllowedSort).ToList();
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public void ApplySorting_NullAllowed_DoesNotThrowForValidField()
    {
        // null allowed means no restriction
        var result = Data().ApplySorting("Name", "asc", null).ToList();
        Assert.Equal("Cactus", result[0].Name);
    }
}

// ============================================================
//  APPLY SEARCH TESTS
// ============================================================

public class ApplySearchTests
{
    private static IQueryable<TestEntity> Data() => new List<TestEntity>
    {
        new() { Id = 1, Name = "Rose Garden",   Category = "Flowers" },
        new() { Id = 2, Name = "Rosemary Herb", Category = "Herbs"   },
        new() { Id = 3, Name = "Blue Orchid",   Category = "Flowers" },
        new() { Id = 4, Name = "Cactus",        Category = "Desert"  },
    }.AsQueryable();

    private static readonly HashSet<string> AllowedSearch = ["Name", "Category"];

    [Fact]
    public void ApplySearch_NullSearch_ReturnsAllRows()
    {
        var result = Data().ApplySearch(null, new List<string> { "Name" }, AllowedSearch).ToList();
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void ApplySearch_EmptySearch_ReturnsAllRows()
    {
        var result = Data().ApplySearch("   ", new List<string> { "Name" }, AllowedSearch).ToList();
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void ApplySearch_NullColumns_ReturnsAllRows()
    {
        var result = Data().ApplySearch("rose", null, AllowedSearch).ToList();
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void ApplySearch_EmptyColumns_ReturnsAllRows()
    {
        var result = Data().ApplySearch("rose", new List<string>(), AllowedSearch).ToList();
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void ApplySearch_SingleColumn_ReturnsMatchingRows()
    {
        var result = Data()
            .ApplySearch("Rose", new List<string> { "Name" }, AllowedSearch)
            .ToList();
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Contains("Rose", r.Name, StringComparison.Ordinal));
    }

    [Fact]
    public void ApplySearch_MultipleColumns_UsesOrLogic()
    {
        // "Flowers" exists only in Category, not in Name
        var result = Data()
            .ApplySearch("Flowers", new List<string> { "Name", "Category" }, AllowedSearch)
            .ToList();
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal("Flowers", r.Category));
    }

    [Fact]
    public void ApplySearch_SearchTrimsWhitespace()
    {
        var result = Data()
            .ApplySearch("  Rose  ", new List<string> { "Name" }, AllowedSearch)
            .ToList();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void ApplySearch_DisallowedColumn_ThrowsValidationException()
    {
        Assert.Throws<ValidationException>(() =>
            Data()
                .ApplySearch("rose", new List<string> { "Secret" }, AllowedSearch)
                .ToList());
    }

    [Fact]
    public void ApplySearch_NoResults_ReturnsEmptyList()
    {
        var result = Data()
            .ApplySearch("ZZZNOMATCH", new List<string> { "Name", "Category" }, AllowedSearch)
            .ToList();
        Assert.Empty(result);
    }
}

// ============================================================
//  INTEGRATION: FILTER + SORT + SEARCH COMBINED
// ============================================================

public class CombinedQueryTests
{
    private static IQueryable<TestEntity> Data() => new List<TestEntity>
    {
        new() { Id = 1, Name = "Rose Garden",   Category = "Flowers", IsActive = true,  Price = 10.5m },
        new() { Id = 2, Name = "Rosemary Herb", Category = "Herbs",   IsActive = true,  Price = 5.0m  },
        new() { Id = 3, Name = "Blue Orchid",   Category = "Flowers", IsActive = false, Price = 20.0m },
        new() { Id = 4, Name = "Cactus",        Category = "Desert",  IsActive = true,  Price = 8.0m  },
        new() { Id = 5, Name = "Jasmine",       Category = "Flowers", IsActive = false, Price = 15.0m },
    }.AsQueryable();

    private static readonly HashSet<string> AllowedFilters = ["Id", "Name", "Category", "IsActive", "Price"];
    private static readonly HashSet<string> AllowedSort = ["Id", "Name", "Price"];
    private static readonly HashSet<string> AllowedSearch = ["Name", "Category"];

    [Fact]
    public void Combined_SearchThenFilterThenSort_ReturnsCorrectResult()
    {
        // Search for "flowers" in Category → 3 rows
        // Filter IsActive == true → 1 row (Rose Garden)
        // Sort by Price desc
        var result = Data()
            .ApplySearch("Flowers", new List<string> { "Category" }, AllowedSearch)
            .ApplyFilters(
                new List<FilterDescriptor> { new() { Key = "IsActive", Operator = "==", Value = "true" } },
                AllowedFilters)
            .ApplySorting("Price", "desc", AllowedSort)
            .ToList();

        Assert.Single(result);
        Assert.Equal("Rose Garden", result[0].Name);
    }

    [Fact]
    public void Combined_NoFilters_NoSearch_JustSort_ReturnsAllSorted()
    {
        var result = Data()
            .ApplyFilters(null, AllowedFilters)
            .ApplySorting("Price", "asc", AllowedSort)
            .ToList();

        Assert.Equal(5, result.Count);
        for (int i = 1; i < result.Count; i++)
            Assert.True(result[i].Price >= result[i - 1].Price);
    }

    [Fact]
    public void Combined_FilterAndBetween_ReturnsIntersection()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { Key = "Price", Operator = "between", Value = "8",    ValueTo = "15" },
            new() { Key = "IsActive", Operator = "==",   Value = "true" },
        };

        var result = Data()
            .ApplyFilters(filters, AllowedFilters)
            .ApplySorting("Price", "asc", AllowedSort)
            .ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Cactus", result[0].Name);
        Assert.Equal("Rose Garden", result[1].Name);
    }
}