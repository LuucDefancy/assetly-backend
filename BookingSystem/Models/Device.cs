using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Models
{
    public class Device
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Description { get; set; }

        [Required]
        [MaxLength(30)]
        public required string Category { get; set; }

        [MaxLength(40)]
        public string? SerialNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public required string Condition { get; set; }

        [Required]
        [MaxLength(20)]
        public required string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        
        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; } = null;

        public int? UpdatedBy { get; set; }

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
