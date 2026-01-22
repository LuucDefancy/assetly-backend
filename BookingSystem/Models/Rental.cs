using BookingSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Models
{
    public class Rental
    {
        public int Id { get; set; }

        [Required]
        public int DeviceId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime RentalDate { get; set; }

        [Required]
        public DateTime ExpectedReturnDate { get; set; }

        public DateTime? ActualReturnDate { get; set; }

        public DateTime? ApprovedAt { get; set; }

        [Required]
        [MaxLength(20)]
        public required RentalStatus Status { get; set; }

        [Required]
        [MaxLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public Device? Device { get; set; }

        public User? User { get; set; }

    }
}
