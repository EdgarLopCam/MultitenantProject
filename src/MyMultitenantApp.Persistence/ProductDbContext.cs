namespace MyMultitenantApp.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using MyMultitenantApp.Domain.Models;

    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Organization> Organizations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Organization>()
                .Property(o => o.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
