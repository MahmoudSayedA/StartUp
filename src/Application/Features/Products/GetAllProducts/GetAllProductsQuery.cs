using Application.Common.Abstractions.Collections;
using Application.Common.Models;
using Application.Features.Products.Models;
using Application.Features.Products.Services;

namespace Application.Features.Products.GetAllProducts;

public class GetAllProductsQuery : HasPagination, ICommand<PaginatedList<ProductModel>>
{
}

internal class GetAllProductsQueryHandler(IProductService repository) : ICommandHandler<GetAllProductsQuery, PaginatedList<ProductModel>>
{
    public async Task<PaginatedList<ProductModel>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await repository.GetAllAsync(request.PageSize, request.PageNumber, cancellationToken);

        return products;
    }

}

