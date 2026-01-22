using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        public required string LastName { get; set; }

        [Required]
        [MaxLength(25)]
        public required string Username { get; set; }

        [Required]
        [MaxLength(200)]
        public required string PasswordHash { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public required string Email { get; set; }

        [Required]
        [MaxLength(25)]
        public required string PhoneNumber { get; set; }

        public required DateTime? Birthday { get; set; }

        public string Role { get; set; } = "User";

        public Boolean EmailConfirmed { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastLoggedIn { get; set; }

        public Boolean ReceiveNotification { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
