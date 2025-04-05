using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CarMarketplace.Data;
using CarMarketplace.DTOs;
using CarMarketplace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CarMarketplace.Services
{
    //Dependency injection via parameters
    public class AuthService(AppDbContext context, IConfiguration configuration, ILogger<AuthService> logger) : IAuthService
    {
        //Login method
        public async Task<TokenResponseDto?> LoginAsync(LoginDto request)
        {
            logger.LogInformation("Login attempt for {Email}", request.Email);

            //Search for user by email
            var user = await context.Users.FirstOrDefaultAsync(e => e.Email == request.Email);
            if (user == null)
            {
                logger.LogWarning("Login attempt failed for {Email}", request.Email);
                return null;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password!) ==
                PasswordVerificationResult.Failed)
            {
                logger.LogWarning("Login attempt failed for {Email}", request.Email);
                return null;
            }

            //If the password is correct, it logs in successfully and generates the access and refresh tokens
            logger.LogInformation("Login successful for {Email}", request.Email);
            return await CreateTokenResponse(user);
            
        }
        //Create response containing the access token and refresh
        private async Task<TokenResponseDto?> CreateTokenResponse(User? user)
        {
            return new TokenResponseDto()
            {
                AccessToken = CreateToken(user),
                RefreshToken = (await GenerateAndSaveRefreshTokenAsync(user))!
            };
        }
        //Register method
        public async Task<User?> RegisterAsync(RegisterDto request)
        {
            logger.LogInformation("Register attempt for {Email}", request.Email);

            //Normalizes email to avoid duplication due to case differences or spaces
            var normalizedEmail = request.Email.Trim().ToLower();

            if (await context.Users.AnyAsync(e => e.Email == normalizedEmail))
            {
                logger.LogWarning("Email already exists: {Email}", request.Email);
                return null;
            }

            if(string.IsNullOrEmpty(request.Role))
            {
                logger.LogWarning("Role is required!");
                return null;
            }

            //List valid roles
            var validRoles = new[] { "Admin", "Seller" ,"Buyer" };
            var normalizedRole = validRoles.FirstOrDefault(r => //Checks if the role entered is in the list of valid roles 
                r.Equals(request.Role.Trim(), StringComparison.OrdinalIgnoreCase));

            if(normalizedRole == null)
            {
                logger.LogWarning("Invalid role: {Role}", request.Role);
                return null;
            }

            //User object is created and the password is hashed
            var user = new User()
            {
                Name = request.Name.Trim(),
                Email = normalizedEmail,
                Role = normalizedRole,
                CreatedAt = DateTime.UtcNow,
                Cars = new List<Car>(),
                Reviews = new List<Review>()
            };

            //Hash the password
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);

            try
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();

                logger.LogInformation("User {Name} registered successfully as {Role}",
                    user.Name, user.Role);

                return user;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during registrations for {Email}", normalizedEmail);
                return null;
            }
        }
        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            logger.LogInformation("Refresh token attempt for user with ID: {UserId}", request.UserId);

            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if(user == null)
            {
                logger.LogWarning("Refresh token failed for user with ID: {UserId}", request.UserId);
                return null;
            }

            return await CreateTokenResponse(user);
        }

        //Refreshing tokens and generating new tokens method
        private async Task<User?> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            logger.LogInformation("Validating refresh token for user with ID: {UserId}", userId);

            var user = await context.Users.FindAsync(userId);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                logger.LogWarning("Invalid refresh token for user with ID: {UserId}", userId);
                return null;
            }

            logger.LogInformation("User with ID: {UserId} found", userId);
            return user;
        }
        //Generate securate refresh token method
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        //Generates and saves the refresh token on the user and updates the expiration date
        private async Task<string?> GenerateAndSaveRefreshTokenAsync(User? user)
        {
            if (user == null)
                return null;
            
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await context.SaveChangesAsync();

            logger.LogInformation("User with email: {Email} generated successfully", user.Email);
            return refreshToken;
        }

        //Logout method
        public async Task<bool> LogoutAsync(int userId)
        {
            logger.LogInformation("Logout attempt for user with ID: {UserId}", userId);
            
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User with ID: {UserId} not found", userId);
                return false;
            }

            //Remove the refresh token and reset its expiration date
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            await context.SaveChangesAsync();
            
            logger.LogInformation("User with ID: {UserId} logged out successfully", userId);
            return true;
        }

        //Create JWT token method
        private string CreateToken(User? user)
        {
            //Claims...
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var tokenKey = configuration.GetValue<string>("AppSettings:Token")
                ?? throw new Exception("Token key is not set in appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            logger.LogInformation("JWT Token created for user with ID: {UserId}", user.Id);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor); //Returns the token as a string
        }
    }
}
