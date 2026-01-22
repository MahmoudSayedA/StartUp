namespace Application.Features.Products.CreateProduct
{
    public class CreateProductValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(p => p.Description)
                .MaximumLength(5000);
        }
    }
}
