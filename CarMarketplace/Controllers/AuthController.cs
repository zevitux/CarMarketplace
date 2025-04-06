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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _authService.RegisterAsync(request);
                if (user == null)
                    return BadRequest("User already exists");

                return Ok(user);
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _authService.LoginAsync(request);
                if (response == null)
                    return BadRequest("Invalid credentials");

                return Ok(response);
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _authService.RefreshTokenAsync(request);
                if (response == null)
                    return Unauthorized("Invalid token");

                return Ok(response);
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var success = await _authService.LogoutAsync(userId);

                return success ? Ok("Logged out successfully") : BadRequest("Logout failed");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }
    }
}