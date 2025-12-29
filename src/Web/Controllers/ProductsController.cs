using Application.Features.Products.CreateProduct;
using Application.Features.Products.GetAllProducts;
using Application.Features.Products.GetById;


namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("all")]
        public async Task<IActionResult> GetProducts(GetAllProductsQuery query)
        {
            var res = await mediator.Send(query);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [EnableRateLimiting(RateLimiterPolicies.MainPolicy)]
        public async Task<IActionResult> GetById(string id)
        {
            if (!Ulid.TryParse(id, out var parsed)) return NotFound();
            var query = new GetProductByIdQuery() { Id = parsed};
            var res = await mediator.Send(query);
            return Ok(res);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command)
        {
            var res = await mediator.Send(command);
            return Ok(res);
        }
    }
}
