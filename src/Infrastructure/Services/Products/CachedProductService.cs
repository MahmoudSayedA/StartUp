using Application.Common.Abstractions.Caching;
using Application.Common.Models;
using Application.Features.Products.Models;
using Application.Features.Products.Services;

namespace Infrastructure.Services.Products;
internal class CachedProductService(ProductService _decorated, ICacheService cacheService) : IProductService
{
    public async Task<PaginatedList<ProductModel>> GetAllAsync(int pageSize, int pageNumber, CancellationToken cancellationToken)
    {
        string masterKey = "products";
        long cacheVersion = await cacheService.GetVersionAsync(masterKey, cancellationToken);
        string key = $"{masterKey}:v{cacheVersion}:size:{pageSize}:page:{pageNumber}";

        var cached = await cacheService.GetDataAsync<PaginatedList<ProductModel>>(key, cancellationToken);
        if (cached != null)
            return cached;

        var productModels = await _decorated.GetAllAsync(pageSize, pageNumber, cancellationToken);

        await cacheService.SetDataAsync(key, productModels, cancellationToken);
        return productModels;

    }

    public async Task<Product?> GetByIdAsync(Ulid id, CancellationToken cancellationToken)
    {
        string key = $"product:{id}";

        Product? cached = await cacheService.GetDataAsync<Product>(key, cancellationToken);
        if (cached != null)
            return cached;

        var product = await _decorated.GetByIdAsync(id, cancellationToken);

        await cacheService.SetDataAsync(key, product, cancellationToken);
        return product;
    }
}
