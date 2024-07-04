namespace MyMultitenantApp.Infrastructure.Services
{
    using System.Threading.Tasks;

    public interface IMigrationService
    {
        Task EnsureProductDatabaseCreatedAsync(string tenant);
    }
}

namespace MyMultitenantApp.Infrastructure.Services
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using MyMultitenantApp.Persistence;

    public class MigrationService : IMigrationService
    {
        private readonly IConfiguration _configuration;

        public MigrationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnsureProductDatabaseCreatedAsync(string tenant)
        {
            var connectionString = _configuration.GetConnectionString("ProductConnection").Replace("{tenant}", tenant);
            var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());

            using (var productDbContext = new ProductDbContext(optionsBuilder.Options))
            {
                await productDbContext.Database.EnsureCreatedAsync();
            }
        }
    }
}
