using Application.Common.Abstractions.Collections;
using Application.Common.Exceptions;
using System.Reflection;

namespace Application.Unit.Tests.common.Abstraction;

public class HasTableViewTests
{
    // setup
    private readonly HasTableView _hasTableView;

    public HasTableViewTests()
    {
        _hasTableView = new TestHasTableView();
    }

    [Fact]
    public void ValidateFilters_WithNotAllowedValues_ThrowValidationException()
    {
        // Arrange
        List<string>? allowedFilters = ["Name", "Age", "Country"];
        _hasTableView.Filters = new Dictionary<string, string>
        {
            { "InvalidFilter", "Value" }
        };
        MethodInfo methodInfo = typeof(HasTableView).GetMethod("ValidateFilters", BindingFlags.NonPublic | BindingFlags.Instance)!;
        // Act & Assert
        Assert.Throws<TargetInvocationException>(() => methodInfo.Invoke(_hasTableView, [allowedFilters]));
    }
    [Fact]
    public void ValidateFilters_WithAllowedValues_DoNotThrowValidationException()
    {
        // Arrange
        List<string>? allowedFilters = ["Name", "Age", "Country"];
        _hasTableView.Filters = new Dictionary<string, string>
        {
            { "Name", "Value" }
        };
        MethodInfo methodInfo = typeof(HasTableView).GetMethod("ValidateFilters", BindingFlags.NonPublic | BindingFlags.Instance)!;
        // Act & Assert
        var exception = Record.Exception(() => methodInfo.Invoke(_hasTableView, [allowedFilters]));
        Assert.Null(exception);
    }

    [Fact]
    public void ValidateSorting_WithNotAllowedValues_ThrowValidationException()
    {
        // Arrange
        List<string>? allowedSorting = ["Name", "CreatedAt"];
        _hasTableView.SortBy = "InvalidSort";

        MethodInfo methodInfo = typeof(HasTableView).GetMethod("ValidateSorting", BindingFlags.NonPublic | BindingFlags.Instance)!;
        // Act & Assert
        Assert.Throws<TargetInvocationException>(() => methodInfo.Invoke(_hasTableView, [allowedSorting]));
    }
    [Fact]
    public void ValidateSorting_WithAllowedValues_DoNotThrowValidationException()
    {
        // Arrange
        List<string>? allowedSorting = ["Name", "CreatedAt"];
        _hasTableView.SortBy = "CreatedAt";

        MethodInfo methodInfo = typeof(HasTableView).GetMethod("ValidateSorting", BindingFlags.NonPublic | BindingFlags.Instance)!;
        // Act & Assert
        var exception = Record.Exception(() => methodInfo.Invoke(_hasTableView, [allowedSorting]));
        Assert.Null(exception);
    }

    [Fact]
    public void VlidateFiltersAndSorting_WithNotAllowedValues_ThrowValidationException()
    {
        // Arrange
        List<string>? allowedFilters = ["Name", "Age", "Country"];
        List<string>? allowedSorting = ["Name", "CreatedAt"];
        _hasTableView.Filters = new Dictionary<string, string>
        {
            { "InvalidFilter", "Value" }
        };
        _hasTableView.SortBy = "InvalidSort";
        // Act & Assert
        Assert.Throws<ValidationException>(() => _hasTableView.ValidateFiltersAndSorting(allowedFilters, allowedSorting));
    }
    [Fact]
    public void VlidateFiltersAndSorting_WithAllowedValues_DoNotThrowValidationException()
    {
        // Arrange
        List<string>? allowedFilters = ["Name", "Age", "Country"];
        List<string>? allowedSorting = ["Name", "CreatedAt"];
        _hasTableView.Filters = new Dictionary<string, string>
        {
            { "Name", "Value" }
        };
        _hasTableView.SortBy = "CreatedAt";
        // Act & Assert
        var exception = Record.Exception(() => _hasTableView.ValidateFiltersAndSorting(allowedFilters, allowedSorting));
        Assert.Null(exception);
    }

}

internal class TestHasTableView : HasTableView
{
}