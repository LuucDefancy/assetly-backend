using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Models
{
    public class Category
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public required string  Name { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Description { get; set; }

        [Required]
        [MaxLength(100)]
        public required string MdiIcon { get; set; }
    }
}
