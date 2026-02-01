using BookingSystem.Data;
using BookingSystem.DTOs;
using BookingSystem.Models;
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
        public DeviceController(FirstNetAPIContext context)
        {
            _context = context;
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
        public async Task<ActionResult<Device>> CreateDevice(Device device)
        {
            if (device == null)
            {
                return BadRequest();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = _context.Users.FirstOrDefault(user => user.Id == int.Parse(userId));
            
            if (user == null && userId != null)
            {
                return NotFound("User does not exist");
            }

            device.CreatedBy = int.Parse(userId);
            Console.WriteLine(device.CreatedBy);
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSpecificDeviceById), new { id = device.Id }, device);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDevice(int id, Device updatedDevice)
        {
            var device = await _context.Devices.FindAsync(id);
                //devices.FirstOrDefault(device => device.Id == id);

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

            device.Name = updatedDevice.Name;
            device.Description = updatedDevice.Description;
            device.Category = updatedDevice.Category;
            device.Condition = updatedDevice.Condition;
            device.Status = updatedDevice.Status;
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

            _context.Devices.Remove(device);

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
