using Application.Common.Abstractions.Caching;
using Domain.Events.ProductEvents;


namespace Application.Features.Products.EventHandlers;
public class ProductChangedRemoveCacheEventHandler(ICacheService cache) : IEventHandler<ProductCreatedEvent>, IEventHandler<ProductUpdatedEvent>, IEventHandler<ProductDeletedEvent>
{
    private readonly ICacheService _cache = cache;

    public async Task Handle(
        ProductCreatedEvent notification,
        CancellationToken cancellationToken)
    {
        await RemoveCacheAsync(notification.Product.Id.ToString(), cancellationToken);

    }

    public async Task Handle(ProductUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await RemoveCacheAsync(notification.Product.Id.ToString(), cancellationToken);
    }

    public async Task Handle(ProductDeletedEvent notification, CancellationToken cancellationToken)
    {
        await RemoveCacheAsync(notification.Id.ToString(), cancellationToken);
    }

    private async Task RemoveCacheAsync(string productId ,CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(
            $"product:{productId}",
            cancellationToken);

        // increment version for invalidation
        await _cache.IncrementVersionAsync("products", cancellationToken);
    }
}
