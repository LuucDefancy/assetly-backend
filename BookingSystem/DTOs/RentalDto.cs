using BookingSystem.Enums;
using BookingSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.DTOs
{
    public class RentalDto
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int UserId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public RentalStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public DeviceDto? Device { get; set; }
        public UserMiniDto? User { get; set; }

        public static RentalDto FromMap(Rental rental, bool includeDeviceRentals = false)
        {
            return new RentalDto
            {
                Id = rental.Id,
                DeviceId = rental.DeviceId,
                UserId = rental.UserId,
                RentalDate = rental.RentalDate,
                ExpectedReturnDate = rental.ExpectedReturnDate,
                ActualReturnDate = rental.ActualReturnDate,
                ApprovedAt = rental.ApprovedAt,
                Status = rental.Status,
                Notes = rental.Notes,
                CreatedAt = rental.CreatedAt,
                UpdatedAt = rental.UpdatedAt,
                Device = DeviceDto.FromMap(rental.Device, includeDeviceRentals),
                User = UserMiniDto.FromMap(rental.User)
            };
        }
    }

    public class RentalMiniDto
    {
        public int Id { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public RentalStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserMiniDto? User { get; set; }

        public static RentalMiniDto FromMap(Rental rental)
        {
            return new RentalMiniDto
            {
                Id = rental.Id,
                RentalDate = rental.RentalDate,
                ExpectedReturnDate = rental.ExpectedReturnDate,
                Status = rental.Status,
                User = UserMiniDto.FromMap(rental.User)
            };
        }
    }

}
