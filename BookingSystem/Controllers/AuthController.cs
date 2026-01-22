using BookingSystem.Data;
using BookingSystem.DTOs;
using BookingSystem.Models;
using BookingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly FirstNetAPIContext _context;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthController(
            FirstNetAPIContext context,
            JwtService jwtService,
            IConfiguration configuration)
        {
            _context = context;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            // Validierung
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Prüfe ob Username bereits existiert
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest(new { message = "Username ist bereits vergeben" });
            }

            // Prüfe ob Email bereits existiert
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest(new { message = "Email ist bereits registriert" });
            }

            // Erstelle neuen User
            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Username = registerDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Birthday = registerDto.Birthday,
                Role = "User",
                EmailConfirmed = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ReceiveNotification = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generiere Token
            var (token, expiresAt) = _jwtService.GenerateToken(user);

            var auth = new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = expiresAt
            };

            return Ok(auth);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            // Validierung
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Finde User
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
            {
                return Unauthorized(new { message = "Ungültige Anmeldedaten" });
            }

            // Prüfe ob User gelöscht wurde
            if (user.DeletedAt != null)
            {
                return Unauthorized(new { message = "Dieser Account wurde deaktiviert" });
            }

            // Prüfe Passwort
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Ungültige Anmeldedaten" });
            }

            // Update LastLoggedIn
            user.LastLoggedIn = DateTime.Now;
            await _context.SaveChangesAsync();

            // Generiere Token
            var (token, expiresAt) = _jwtService.GenerateToken(user);

            var auth = new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = expiresAt
            };

            return Ok(auth);
        }

        [HttpPost("validate")]
        public ActionResult ValidateToken([FromBody] string token)
        {
            var isValid = _jwtService.ValidateToken(token);

            if (isValid)
            {
                return Ok(new { valid = true, message = "Token ist gültig" });
            }

            return Unauthorized(new { valid = false, message = "Token ist ungültig oder abgelaufen" });
        }
    }
}