namespace MyMultitenantApp.Api.Controllers
{
    using MediatR;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using MyMultitenantApp.Application.Commands.CreateProduct;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;
    using MyMultitenantApp.Persistence;

    [Route("api/{tenant}/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            var tenant = HttpContext.Items["tenant"]?.ToString();
            if (tenant == null)
                return BadRequest("Invalid tenant");

            var productDbContext = HttpContext.Items["ProductDbContext"] as ProductDbContext;
            if (productDbContext == null)
                return StatusCode(500, "Product database context not available");

            command.OrganizationId = await GetOrganizationIdBySlug(tenant, productDbContext);
            if (command.OrganizationId == 0)
                return BadRequest("Invalid organization");

            var productId = await _mediator.Send(command);
            return Ok(productId);
        }

        private async Task<int> GetOrganizationIdBySlug(string slug, ProductDbContext productDbContext)
        {
            var organization = await productDbContext.Organizations.SingleOrDefaultAsync(o => o.SlugTenant == slug);
            return organization?.Id ?? 0;
        }
    }
}
