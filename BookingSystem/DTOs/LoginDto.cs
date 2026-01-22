
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username ist erforderlich")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Passwort ist erforderlich")]
        public string Password { get; set; }
    }
}