using Application.Common.Abstractions.Data;
using Domain.Events.ProductEvents;

namespace Application.Features.Products.CreateProduct;

public class CreateProductCommand : ICommand<string>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

internal class CreateProductCommandHandler(IApplicationDbContext context) : ICommandHandler<CreateProductCommand, string>
{
    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        Product product = new()
        {
            Name = request.Name,
            Description = request.Description
        };


        await context.Set<Product>().AddAsync(product, cancellationToken);

        product.AddDomainEvent(new ProductCreatedEvent(product));

        await context.SaveChangesAsync(cancellationToken);

        return product.Id.ToString();
    }
}
