namespace MyMultitenantApp.Application.Services
{
    using System.Threading.Tasks;
    using MyMultitenantApp.Domain.Models;
    using MyMultitenantApp.Persistence;
    using Microsoft.EntityFrameworkCore;
    using MyMultitenantApp.Application.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using MyMultitenantApp.Persistence.Migrations;

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UserService(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<(bool Succeeded, string[] Errors, User User)> RegisterAsync(RegisterRequest request, string tenant)
        {
            var existingUser = await _applicationDbContext.Users
                .SingleOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)
            {
                return (false, new[] { "User with this email already exists" }, null);
            }

            var organization = await _applicationDbContext.Organizations
                .SingleOrDefaultAsync(o => o.SlugTenant == tenant);

            if (organization == null)
            {
                return (false, new[] { "Organization not found" }, null);
            }

            var user = new User
            {
                Email = request.Email,
                Password = request.Password,
                OrganizationId = organization.Id
            };

            _applicationDbContext.Users.Add(user);
            await _applicationDbContext.SaveChangesAsync();

            return (true, new string[] { }, user);
        }

        public async Task<(bool Succeeded, string[] Errors, string Token)> LoginAsync(LoginRequest request, string tenant)
        {
            var organization = await _applicationDbContext.Organizations
                .SingleOrDefaultAsync(o => o.SlugTenant == tenant);

            if (organization == null)
            {
                return (false, new[] { "Organization not found" }, null);
            }

            var user = await _applicationDbContext.Users
                .SingleOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password && u.OrganizationId == organization.Id);

            if (user == null)
            {
                return (false, new[] { "Invalid credentials" }, null);
            }

            // Generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return (true, new string[] { }, tokenString);
        }
    }
}
