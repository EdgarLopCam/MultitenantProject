namespace MyMultitenantApp.Application.Services
{
    using System.Threading.Tasks;
    using MyMultitenantApp.Domain.Models;
    using MyMultitenantApp.Persistence;
    using MyMultitenantApp.Infrastructure.Services;
    using MyMultitenantApp.Persistence.Migrations;

    public class OrganizationService : IOrganizationService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMigrationService _migrationService;

        public OrganizationService(ApplicationDbContext applicationDbContext, IMigrationService migrationService)
        {
            _applicationDbContext = applicationDbContext;
            _migrationService = migrationService;
        }

        public async Task<Organization> CreateOrganizationAsync(string name, string slugTenant)
        {
            var organization = new Organization
            {
                Name = name,
                SlugTenant = slugTenant
            };

            _applicationDbContext.Organizations.Add(organization);
            await _applicationDbContext.SaveChangesAsync();

            // Create the product database for the new tenant
            await _migrationService.EnsureProductDatabaseCreatedAsync(slugTenant);

            return organization;
        }
    }
}
