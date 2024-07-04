namespace MyMultitenantApp.Application.Services
{
    using System.Threading.Tasks;
    using MyMultitenantApp.Domain.Models;

    public interface IOrganizationService
    {
        Task<Organization> CreateOrganizationAsync(string name, string slugTenant);
    }
}
