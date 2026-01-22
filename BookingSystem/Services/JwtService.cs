using BookingSystem.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace BookingSystem.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly JsonWebTokenHandler _tokenHandler;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _tokenHandler = new JsonWebTokenHandler();
        }

        public (string Token, DateTime ExpiresAt) GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var expiryMinutes = Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"]);
            var expiresAt = now.AddMinutes(expiryMinutes);
            Console.WriteLine(now);
            Console.WriteLine(expiresAt);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = expiresAt,
                IssuedAt = now,
                NotBefore = now,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = credentials
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return (token, expiresAt);
        }

        public bool ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var result = _tokenHandler.ValidateToken(token, validationParameters);
                return result.IsValid;
            }
            catch
            {
                return false;
            }
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = false // Bei Refresh Tokens auf false setzen
            };

            try
            {
                var result = _tokenHandler.ValidateToken(token, validationParameters);
                if (result.IsValid && result.ClaimsIdentity != null)
                {
                    return new ClaimsPrincipal(result.ClaimsIdentity);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}