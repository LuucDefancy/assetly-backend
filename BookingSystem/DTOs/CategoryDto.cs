using BookingSystem.Enums;
using BookingSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string MdiIcon { get; set; }

        public static CategoryDto FromMap(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                MdiIcon = category.MdiIcon
            };
        }
    }
}
