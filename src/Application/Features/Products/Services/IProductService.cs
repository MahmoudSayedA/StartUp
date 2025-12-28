using Application.Common.Models;
using Application.Features.Products.Models;

namespace Application.Features.Products.Services;

public interface IProductService
{
    Task<Product?> GetByIdAsync(Ulid id, CancellationToken cancellationToken);
    Task<PaginatedList<ProductModel>> GetAllAsync(int pageSize, int pageNumber, CancellationToken cancellationToken);

}