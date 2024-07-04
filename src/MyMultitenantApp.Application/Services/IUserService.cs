namespace MyMultitenantApp.Application.Services
{
    using MyMultitenantApp.Application.Models;
    using MyMultitenantApp.Domain.Models;
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<(bool Succeeded, string[] Errors, User User)> RegisterAsync(RegisterRequest request, string tenant);
        Task<(bool Succeeded, string[] Errors, string Token)> LoginAsync(LoginRequest request, string tenant);
    }
}
