namespace Application.Features.Products.Models;

public record ProductModel
{
    public Ulid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? CreatedAt {  get; set; }

    public ProductModel() { }
    public ProductModel(Product product)
    {
        Id = product.Id;
        Name = product.Name;
        Description = product.Description;
        CreatedBy = product.CreatedBy;
        CreatedAt = product.CreatedAt;
    }
}
