using BookingSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.DTOs
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string SerialNumber { get; set; }
        public string Condition { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public string? ImageUrl { get; set; }

        public CategoryDto? Category { get; set; }

        public List<RentalMiniDto>? Rentals { get; set; } = new List<RentalMiniDto>();


        public static DeviceDto FromMap(Device device, bool includeRentals = false)
        {
            return new DeviceDto
            {
                Id = device.Id,
                Name = device.Name,
                Description = device.Description,
                CategoryId = device.CategoryId,
                SerialNumber = device.SerialNumber,
                Condition = device.Condition,
                Status = device.Status,
                CreatedAt = device.CreatedAt,
                CreatedBy = device.CreatedBy,
                UpdatedAt = device.UpdatedAt,
                ImageUrl = !string.IsNullOrEmpty(device.ImageFileName)
                ? $"/api/device/image/{device.ImageFileName}"
                : null,
                Category = device.Category != null ? CategoryDto.FromMap(device.Category) : null,
                Rentals = includeRentals && device.Rentals != null
            ? device.Rentals.Select(rental => RentalMiniDto.FromMap(rental)).ToList()
            : new List<RentalMiniDto>()
            };
        }
    }
}
