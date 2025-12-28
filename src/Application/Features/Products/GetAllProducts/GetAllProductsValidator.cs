namespace Application.Features.Products.GetAllProducts
{
    public class GetAllProductsValidator : AbstractValidator<GetAllProductsQuery>
    {
        public GetAllProductsValidator()
        {
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(r => r.PageSize).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
        }
    }
}
