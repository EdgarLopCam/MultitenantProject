namespace MyMultitenantApp.Api.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MyMultitenantApp.Application.Services;
    using Microsoft.Extensions.Configuration;
    using MyMultitenantApp.Persistence;
    using Microsoft.EntityFrameworkCore;
    using MyMultitenantApp.Domain.Models;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        private readonly IConfiguration _configuration;

        public OrganizationsController(IOrganizationService organizationService, IConfiguration configuration)
        {
            _organizationService = organizationService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrganizationRequest request)
        {
            var organization = await _organizationService.CreateOrganizationAsync(request.Name, request.SlugTenant);

            // Create and seed the tenant database
            var tenant = request.SlugTenant;
            var connectionString = _configuration.GetConnectionString("ProductConnection").Replace("{tenant}", tenant);
            var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());

            using (var productDbContext = new ProductDbContext(optionsBuilder.Options))
            {
                await productDbContext.Database.EnsureCreatedAsync();
                if (!await productDbContext.Organizations.AnyAsync(o => o.SlugTenant == organization.SlugTenant))
                {
                    var newOrganization = new Organization
                    {
                        Name = organization.Name,
                        SlugTenant = organization.SlugTenant
                    };
                    productDbContext.Organizations.Add(newOrganization);
                    await productDbContext.SaveChangesAsync();
                }
            }

            return Ok(organization);
        }
    }

    public class CreateOrganizationRequest
    {
        public string Name { get; set; }
        public string SlugTenant { get; set; }
    }
}
