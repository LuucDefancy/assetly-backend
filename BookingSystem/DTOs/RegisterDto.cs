
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Vorname ist erforderlich")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nachname ist erforderlich")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username ist erforderlich")]
        [MinLength(3, ErrorMessage = "Username muss mindestens 3 Zeichen lang sein")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Passwort ist erforderlich")]
        [MinLength(6, ErrorMessage = "Passwort muss mindestens 6 Zeichen lang sein")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email ist erforderlich")]
        [EmailAddress(ErrorMessage = "Ungültige Email-Adresse")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? Birthday { get; set; }
    }
}