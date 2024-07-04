namespace MyMultitenantApp.Application.Queries.GetProducts
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using MyMultitenantApp.Domain.Models;
    using MyMultitenantApp.Persistence;

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<Product>>
    {
        private readonly IServiceProvider _serviceProvider;

        public GetProductsQueryHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IEnumerable<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var productDbContext = httpContextAccessor.HttpContext?.Items["ProductDbContext"] as ProductDbContext;

            if (productDbContext == null)
            {
                throw new InvalidOperationException("ProductDbContext is not available.");
            }

            return await productDbContext.Products.ToListAsync(cancellationToken);
        }
    }
}
