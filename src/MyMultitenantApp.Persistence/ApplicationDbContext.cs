namespace MyMultitenantApp.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using MyMultitenantApp.Domain.Models;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organization>()
                .Property(o => o.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            // Agregar datos por defecto a la tabla Organizations
            modelBuilder.Entity<Organization>().HasData(
                new Organization { Id = 1, Name = "Organization1", SlugTenant = "default_tenant" }
            );
        }
    }
}
