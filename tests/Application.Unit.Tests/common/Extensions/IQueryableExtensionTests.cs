using Application.Common.Extensions;

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
        var filters = new Dictionary<string, string>
        {
            { "Name", "A" }
        };
        // Act
        var result = data.ApplyFilters(filters).ToList();
        // Assert
        Assert.Single(result);
        Assert.Equal("Alice", result[0].Name);
    }

    [Fact]
    public void ApplySorting_ShouldSortQueryable_WhenSortParametersAreProvided()
    {
        // Arrange
        var data = new List<TestEntity>
        {
            new TestEntity { Name = "Charlie" },
            new TestEntity { Name = "Alice" },
            new TestEntity { Name = "Bob" }
        }.AsQueryable();

        // Act
        var result = data.ApplySorting("Name", "asc").ToList();

        // Assert
        Assert.Collection(result,
            item => Assert.Equal("Alice", item.Name),
            item => Assert.Equal("Bob", item.Name),
            item => Assert.Equal("Charlie", item.Name));
    }


}