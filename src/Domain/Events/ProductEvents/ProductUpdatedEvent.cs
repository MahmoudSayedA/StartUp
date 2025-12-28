using Domain.Entities;

namespace Domain.Events.ProductEvents
{
    public class ProductUpdatedEvent : BaseEvent
    {
        public Product Product { get; set; }
        public ProductUpdatedEvent(Product product) => Product = product;

    }
}
