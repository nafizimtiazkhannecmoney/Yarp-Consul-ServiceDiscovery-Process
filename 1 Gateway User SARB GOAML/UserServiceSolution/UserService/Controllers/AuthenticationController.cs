/**
* Description : AuthenticationController class for handling user authentication requests.
* @author     : Nafiz Imtiaz khan
* @since      : 07/05/2025      
*/

using Microsoft.AspNetCore.Mvc;
using UserService.Model;
using UserService.Repository;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(AuthService authService, ILogger<AuthenticationController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return result is null
                ? Unauthorized(new { message = "Invalid username or password" })
                : Ok(result);                          // { token, user {...} }
        }
    }
}
