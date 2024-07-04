namespace MyMultitenantApp.Application.Queries.GetProducts
{
    using MediatR;
    using MyMultitenantApp.Domain.Models;
    using System.Collections.Generic;

    public class GetProductsQuery : IRequest<IEnumerable<Product>>
    {
        public int OrganizationId { get; set; }
    }
}
