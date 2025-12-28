using Application.Common.Exceptions;
using Application.Features.Products.Models;
using Application.Features.Products.Services;

namespace Application.Features.Products.GetById;

public class GetProductByIdQuery : ICommand<ProductModel>
{
    public Ulid Id { get; set; }
}

public class GetProductByIdQueryHandler(IProductService repository) : ICommandHandler<GetProductByIdQuery, ProductModel>
{
    public async Task<ProductModel> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken);

        return product == null
            ? throw new NotFoundException(nameof(Product), request.Id)
            : new ProductModel(product);
    }
}
