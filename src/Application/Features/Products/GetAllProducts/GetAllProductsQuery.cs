using Application.Common.Abstractions.Collections;
using Application.Common.Abstractions.Data;
using Application.Common.Extensions;
using Application.Common.Models;
using Application.Features.Products.Models;
using Application.Features.Products.Services;

namespace Application.Features.Products.GetAllProducts;

public class GetAllProductsQuery : HasTableViewWithSearch, ICommand<PaginatedListWithCount<ProductModel>>
{
}

internal class GetAllProductsQueryHandler(IApplicationDbContext dbContext) : ICommandHandler<GetAllProductsQuery, PaginatedListWithCount<ProductModel>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    public async Task<PaginatedListWithCount<ProductModel>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // var products = await repository.GetAllAsync(request.PageSize, request.PageNumber, cancellationToken);

        var products = await _dbContext.Set<Product>()
            .AsNoTracking()
            .ApplyFilters(request.Filters, ["id", "name", "createdBy", "createdAt"])
            .ApplySorting(request.SortBy, request.SortDirection, ["id", "name", "createdAt"])
            .ApplySearch(request.Search, request.SearchColumns, ["id", "name", "createdBy", "createdAt", "description"])
            .Select(p => new ProductModel
            {
                Id = p.Id,
                Name = p.Name,
                CreatedAt = p.CreatedAt,
                CreatedBy = p.CreatedBy,
                Description = p.Description,
            })
            .ToPaginatedListWithCountAsync(request.PageNumber, request.PageSize, cancellationToken);

        return products;
    }

}

