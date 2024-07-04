namespace MyMultitenantApp.Api.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using MyMultitenantApp.Application.Services;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(MyMultitenantApp.Application.Models.RegisterRequest request)
        {
            var tenant = request.SlugTenant;
            var result = await _userService.RegisterAsync(request, tenant);
            if (result.Succeeded)
            {
                return Ok(result.User);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(MyMultitenantApp.Application.Models.LoginRequest request)
        {
            var tenant = "default_tenant";
            var result = await _userService.LoginAsync(request, tenant);
            if (result.Succeeded)
            {
                return Ok(new { Token = result.Token });
            }
            return Unauthorized(result.Errors);
        }
    }
}
