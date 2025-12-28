using Domain.Events.ProductEvents;

namespace Application.Features.Products.EventHandlers
{
    internal class ProductCreatedEventHandler : IEventHandler<ProductCreatedEvent>
    {
        public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("this is awesome!");
            await Task.CompletedTask;
        }
    }
}
