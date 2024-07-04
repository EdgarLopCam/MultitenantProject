namespace MyMultitenantApp.Domain.Interfaces
{
    using MyMultitenantApp.Domain.Models;

    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
