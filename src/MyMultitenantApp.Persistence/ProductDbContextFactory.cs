namespace MyMultitenantApp.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class ProductDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public ProductDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ProductDbContext CreateDbContext(string tenant)
        {
            var connectionString = _configuration.GetConnectionString("ProductConnection").Replace("{tenant}", tenant);
            var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
            return new ProductDbContext(optionsBuilder.Options);
        }
    }
}
