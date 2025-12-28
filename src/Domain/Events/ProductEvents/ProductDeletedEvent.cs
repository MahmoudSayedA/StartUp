namespace Domain.Events.ProductEvents
{
    public class ProductDeletedEvent(Ulid id) : BaseEvent
    {
        public Ulid Id { get; set; } = id;
    }
}
