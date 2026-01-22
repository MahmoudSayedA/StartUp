using Application.Features.Products.CreateProduct;
using Application.Features.Products.GetById;
using Microsoft.EntityFrameworkCore;

namespace Application.Integration.Tests.Features;
public class ProductTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    
    [Fact]
    public async Task CreateCommand_WithNameAndDescription_ShouldCreateProduct()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Test Product1",
            Description = "This is a test product."
        };
        // Act
        var productId = await _sender.Send(command);
        // Assert
        var product = await _dbContext.Products.FindAsync(Guid.Parse(productId));
        Assert.NotNull(product);
        Assert.Equal("Test Product1", product!.Name);
        Assert.Equal("This is a test product.", product.Description);
    }
    [Fact]
    public async Task CreateCommand_WithNameOnly_ShouldCreateProduct()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Test Product2"
        };
        // Act
        var productId = await _sender.Send(command);
        // Assert
        var product = await _dbContext.Products.FindAsync(Guid.Parse(productId));
        Assert.NotNull(product);
        Assert.Equal("Test Product2", product!.Name);
        Assert.Null(product.Description);
    }
    [Fact]
    public async Task CreateCommand_WithEmptyName_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = ""
        };
        // Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () =>
        {
            await _sender.Send(command);
        });
    }
    [Fact]
    public async Task CreateCommand_WithNullName_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = null!
        };
        // Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () =>
        {
            await _sender.Send(command);
        });
    }
    [Fact]
    public async Task CreateCommand_WithLongName_ShouldThrowValidationException()
    {
        // Arrange
        var longName = new string('A', 201); // Assuming max length is 200
        var command = new CreateProductCommand
        {
            Name = longName
        };
        // Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () =>
        {
            await _sender.Send(command);
        });
    }
    [Fact]
    public async Task CreateCommand_WithLongDescription_ShouldThrowValidationException()
    {
        // Arrange
        var longDescription = new string('B', 1001); // Assuming max length is 1000
        var command = new CreateProductCommand
        {
            Name = "Valid Name",
            Description = longDescription
        };
        // Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () =>
        {
            await _sender.Send(command);
        });
    }
    [Fact]
    public async Task CreateCommand_ThenGetProduct_ShouldReturnCreatedProduct()
    {
        var products = await _dbContext.Products.ToListAsync();
        Console.WriteLine(products);
        // Arrange
        var createCommand = new CreateProductCommand
        {
            Name = "Test Product3",
            Description = "This is another test product."
        };
        var productId = await _sender.Send(createCommand);
        var getQuery = new GetProductByIdQuery
        {
            Id = Ulid.Parse(productId)
        };
        // Act
        var product = await _sender.Send(getQuery);
        // Assert
        Assert.NotNull(product);
        Assert.Equal("Test Product3", product.Name);
        Assert.Equal("This is another test product.", product.Description);
    }
}
