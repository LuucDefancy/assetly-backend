namespace BookingSystem.DTOs
{
    public class DeviceCreateDto
    {
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string Condition { get; set; }
        public string Status { get; set; }
        public IFormFile? Image { get; set; }
    }

    // DTOs/DeviceUpdateDto.cs
    public class DeviceUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string Condition { get; set; }
        public string Status { get; set; }
        public IFormFile? Image { get; set; }
    }
}
