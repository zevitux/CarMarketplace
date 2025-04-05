using System.Security.Claims;
using CarMarketplace.DTOs;
using CarMarketplace.Models;
using CarMarketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarMarketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]

        public async Task<ActionResult<User>> Register(RegisterDto request)
        {
            var user = await _authService.RegisterAsync(request);
            if (user == null)
                return BadRequest("User already exists");
        
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginDto request)
        {
            var response = await _authService.LoginAsync(request);
            if(response == null)
                return BadRequest("Invalid credentials");

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var response = await _authService.RefreshTokenAsync(request);
            if (response == null)
                return Unauthorized("Invalid token");

            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _authService.LogoutAsync(userId);

            return success ? Ok("Logged out successfully") : BadRequest("Logout failed");
        }
    }
}