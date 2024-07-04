namespace MyMultitenantApp.Api.Middleware
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using MyMultitenantApp.Persistence;
    using MyMultitenantApp.Persistence.Migrations;

    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public TenantMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var pathSegments = context.Request.Path.Value.Split('/');

            // Determinar si la ruta contiene `{tenant}`
            if (pathSegments.Length > 2 && pathSegments[1].Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                if (pathSegments[2].Equals("Auth", StringComparison.OrdinalIgnoreCase) ||
                    pathSegments[2].Equals("Organizations", StringComparison.OrdinalIgnoreCase))
                {
                    // Estos endpoints no tienen `{tenant}` en la URL
                    await _next(context);
                    return;
                }

                // Obtener `{tenant}` de la URL
                var tenant = pathSegments[2];
                context.Items["tenant"] = tenant;

                if (!string.IsNullOrEmpty(tenant))
                {
                    var connectionString = _configuration.GetConnectionString("ProductConnection").Replace("{tenant}", tenant);
                    var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
                    optionsBuilder.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());

                    using (var scope = context.RequestServices.CreateScope())
                    {
                        var productDbContext = new ProductDbContext(optionsBuilder.Options);
                        context.Items["ProductDbContext"] = productDbContext;

                        // Si es un endpoint de productos, no es necesario sincronizar organizaciones.
                        if (pathSegments[3].Equals("Products", StringComparison.OrdinalIgnoreCase))
                        {
                            await _next(context);
                            return;
                        }

                        // Para otros endpoints, asegurar que la tabla de organizaciones exista en el contexto de la base de datos de productos
                        var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var organizations = await applicationDbContext.Organizations.ToListAsync();
                        foreach (var organization in organizations)
                        {
                            if (!await productDbContext.Organizations.AnyAsync(o => o.Id == organization.Id))
                            {
                                productDbContext.Organizations.Add(organization);
                            }
                        }
                        await productDbContext.SaveChangesAsync();
                    }
                }
            }

            await _next(context);
        }
    }
}