using BookingSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public string Role { get; set; }
        public Boolean EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public Boolean ReceiveNotification { get; set; }
        public DateTime? DeletedAt { get; set; }
        public List<RentalDto>? Rentals { get; set; } = new List<RentalDto>();
    }

    public class UserMiniDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public static UserMiniDto FromMap(User user)
        {
            return new UserMiniDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}
