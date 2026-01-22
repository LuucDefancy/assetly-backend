using BookingSystem.Data;
using BookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Schützt alle Endpoints
    public class UserController : ControllerBase
    {
        private readonly FirstNetAPIContext _context;

        public UserController(FirstNetAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] // Nur Admins können alle User sehen
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = await _context.Users
                .Where(u => u.DeletedAt == null)
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("me")]
        public async Task<ActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Ungültiger Token" });
            }

            var userId = int.Parse(userIdClaim);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "Benutzer nicht gefunden" });
            }

            if (user.DeletedAt != null)
            {
                return BadRequest(new { message = "Benutzer wurde gelöscht" });
            }

            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Ungültiger Token" });
            }

            var currentUserId = int.Parse(userIdClaim);
            var isAdmin = User.IsInRole("Admin");

            // User kann nur sich selbst sehen, außer er ist Admin
            if (currentUserId != id && !isAdmin)
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Benutzer nicht gefunden" });
            }

            if (user.DeletedAt != null)
            {
                return BadRequest(new { message = "Benutzer wurde gelöscht" });
            }

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Nur Admins können User anlegen
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Ungültige Benutzerdaten" });
            }

            // Prüfe ob Username bereits existiert
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest(new { message = "Username ist bereits vergeben" });
            }

            // Prüfe ob Email bereits existiert
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest(new { message = "Email ist bereits registriert" });
            }

            // Hash das Passwort falls vorhanden
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            }

            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(User user, int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Ungültiger Token" });
            }

            var currentUserId = int.Parse(userIdClaim);
            var isAdmin = User.IsInRole("Admin");

            // User kann nur sich selbst updaten, außer er ist Admin
            if (currentUserId != id && !isAdmin)
            {
                return Forbid();
            }

            var actualUser = await _context.Users.FindAsync(id);

            if (actualUser == null)
            {
                return NotFound(new { message = "Benutzer nicht gefunden" });
            }

            if (actualUser.DeletedAt != null)
            {
                return BadRequest(new { message = "Benutzer wurde gelöscht" });
            }

            // Update Felder
            actualUser.FirstName = user.FirstName;
            actualUser.LastName = user.LastName;
            actualUser.Username = user.Username;

            // Passwort nur updaten wenn neu gesetzt
            if (!string.IsNullOrEmpty(user.PasswordHash) &&
                user.PasswordHash != actualUser.PasswordHash)
            {
                actualUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            }

            actualUser.Email = user.Email;
            actualUser.PhoneNumber = user.PhoneNumber;
            actualUser.Birthday = user.Birthday;

            // Nur Admins können Role ändern
            if (isAdmin)
            {
                actualUser.Role = user.Role;
                actualUser.EmailConfirmed = user.EmailConfirmed;
            }

            actualUser.UpdatedAt = DateTime.Now;
            actualUser.ReceiveNotification = user.ReceiveNotification;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Nur Admins können User löschen
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Benutzer nicht gefunden" });
            }

            if (user.DeletedAt != null)
            {
                return BadRequest(new { message = "Benutzer ist bereits gelöscht" });
            }

            // Soft Delete
            user.DeletedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            // Oder Hard Delete:
            // _context.Users.Remove(user);
            // await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/restore")]
        [Authorize(Roles = "Admin")] // Nur Admins können User wiederherstellen
        public async Task<ActionResult> RestoreUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Benutzer nicht gefunden" });
            }

            if (user.DeletedAt == null)
            {
                return BadRequest(new { message = "Benutzer ist nicht gelöscht" });
            }

            user.DeletedAt = null;
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Benutzer wurde wiederhergestellt" });
        }
    }
}