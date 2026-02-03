using BookingSystem.Data;
using BookingSystem.DTOs;
using BookingSystem.Models;
using BookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {

        private readonly FirstNetAPIContext _context;
        private readonly IImageStorageService _imageStorage;
        public DeviceController(FirstNetAPIContext context, IImageStorageService imageStorage)
        {
            _context = context;
            _imageStorage = imageStorage;
        }


        [HttpGet]
        public async Task<ActionResult<List<DeviceDto>>> GetDevices()
        {
            var isAdmin = User.IsInRole("Admin");

            IQueryable<Device> query = _context.Devices
     .Include(d => d.Category) // Category laden
     .Include(d => d.Rentals).ThenInclude(r => r.User); // Rentals laden

            if (!isAdmin)
            {
                query = query.Where(device => device.Status == "Verfügbar");
            }

            var devices = await query.ToListAsync();

            var result = devices.Select(device => DeviceDto.FromMap(device)).ToList();

            return Ok(result);
        }

        [HttpGet("image/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "devices");
            var filePath = Path.Combine(uploadsPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var contentType = GetContentType(fileName);
            var image = System.IO.File.OpenRead(filePath);
            return File(image, contentType);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetSpecificDeviceById(int id)
        {
         
            var device = await _context.Devices.Include(device => device.Rentals).ThenInclude(rental => rental.User).FirstOrDefaultAsync(res => res.Id == id);

            if (device == null)
            {
                return NotFound();
            }

            var result = DeviceDto.FromMap(device);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Device>> CreateDevice([FromForm] DeviceCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = _context.Users.FirstOrDefault(user => user.Id == int.Parse(userId));
            
            if (user == null && userId != null)
            {
                return NotFound("User does not exist");
            }

            // Bild speichern, falls vorhanden
            string imageFileName = null;
            if (dto.Image != null)
            {
                try
                {
                    imageFileName = await _imageStorage.SaveImageAsync(dto.Image);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            var device = new Device
            {
                Name = dto.Name,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Condition = dto.Condition,
                SerialNumber = dto.SerialNumber,
                Status = dto.Status,
                ImageFileName = imageFileName,
                CreatedBy = int.Parse(userId)
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSpecificDeviceById), new { id = device.Id }, device);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDevice(int id, [FromForm] DeviceUpdateDto dto)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _context.Users.FirstOrDefault(user => user.Id == int.Parse(userId));

            if (user == null && userId != null)
            {
                return NotFound("User does not exist");
            }

            // Wenn ein neues Bild hochgeladen wird
            if (dto.Image != null)
            {
                // Altes Bild löschen
                if (!string.IsNullOrEmpty(device.ImageFileName))
                {
                    await _imageStorage.DeleteImageAsync(device.ImageFileName);
                }

                // Neues Bild speichern
                try
                {
                    device.ImageFileName = await _imageStorage.SaveImageAsync(dto.Image);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            device.Name = dto.Name;
            device.Description = dto.Description;
            device.CategoryId = dto.CategoryId;
            device.Condition = dto.Condition;
            device.Status = dto.Status;
            device.UpdatedAt = DateTime.Now;
            device.UpdatedBy = user.Id;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            // Bild löschen, falls vorhanden
            if (!string.IsNullOrEmpty(device.ImageFileName))
            {
                await _imageStorage.DeleteImageAsync(device.ImageFileName);
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

    }


}
