namespace MyMultitenantApp.Application.Commands.CreateProduct
{
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using MyMultitenantApp.Domain.Models;
    using MyMultitenantApp.Persistence;

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateProductCommandHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productDbContext = _httpContextAccessor.HttpContext.Items["ProductDbContext"] as ProductDbContext;
            if (productDbContext == null)
                throw new System.Exception("ProductDbContext is not available");

            var product = new Product
            {
                Name = request.Name,
                OrganizationId = request.OrganizationId
            };

            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}
