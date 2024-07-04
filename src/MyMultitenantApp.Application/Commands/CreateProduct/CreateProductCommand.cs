namespace MyMultitenantApp.Application.Commands.CreateProduct
{
    using MediatR;

    public class CreateProductCommand : IRequest<int>
    {
        public string Name { get; set; }
        public int OrganizationId { get; set; }
    }
}
