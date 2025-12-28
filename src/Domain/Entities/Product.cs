namespace Domain.Entities
{
    public class Product : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
