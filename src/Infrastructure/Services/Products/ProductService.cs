using Application.Common.Models;
using Application.Features.Products.Models;
using Application.Features.Products.Services;
using Infrastructure.Data;

namespace Infrastructure.Services.Products;
internal class ProductService(ApplicationDbContext context) : IProductService
{

    public async Task<PaginatedList<ProductModel>> GetAllAsync(int pageSize, int pageNumber, CancellationToken cancellationToken)
    {
        var query = context.Set<Product>().Select(p => new ProductModel(p));

        var products = await PaginatedList<ProductModel>.CreateAsync(query, pageNumber, pageSize, cancellationToken);
        return products;

    }

    public async Task<Product?> GetByIdAsync(Ulid id, CancellationToken cancellationToken)
    {
        var product = await context.Products.FindAsync([id, cancellationToken], cancellationToken);
        return product;
    }
}
