using BookingSystem.Data;
using BookingSystem.DTOs;
using BookingSystem.Enums;
using BookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {

        private readonly FirstNetAPIContext _context;
        public RentalController(FirstNetAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentalDto>>> GetRentals()
        {
            var isAdmin = User.IsInRole("Admin");
            List<Rental> rentals;

            if (isAdmin)
            {
                rentals = await _context.Rentals
                    .Include(r => r.Device)
                        .ThenInclude(d => d.Rentals)
                    .Include(r => r.User)
                    .ToListAsync();
            }
            else
            {
                rentals = await _context.Rentals
                    .AsNoTracking()
                    .Include(r => r.Device)
                    .Include(r => r.User)
                    .ToListAsync();
            }

            var result = rentals.Select(r => RentalDto.FromMap(r, isAdmin)).ToList();
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Rental>> GetSpecificRentalById(int id)
        {
            var rental = await _context.Rentals.Include(rental => rental.Device).Include(rental => rental.User).FirstOrDefaultAsync(res => res.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            var result = RentalDto.FromMap(rental);
            return Ok(result);
        }

        [HttpPost()]
        public async Task<ActionResult<Rental>> RequestRental(Rental rental)
        {
            if(rental == null)
            {
                return BadRequest("Existiert nicht");
            }

            if(rental.Device.Status != "Verfügbar")
            {
                return BadRequest("Status ist ungleich Verfübar");
            }

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSpecificRentalById), new { id = rental.Id }, rental);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRental(int id, Rental updatedRental)
        {
            var rental = await _context.Rentals.FindAsync(id);
            

            if (rental == null)
            {
                return NotFound();
            }

            rental.DeviceId = updatedRental.DeviceId;
            rental.UserId = updatedRental.UserId;
            rental.RentalDate = updatedRental.RentalDate;
            rental.ExpectedReturnDate = updatedRental.ExpectedReturnDate;
            rental.ActualReturnDate = updatedRental.ActualReturnDate;
            rental.ApprovedAt = updatedRental.ApprovedAt;
            rental.Status = updatedRental.Status;
            rental.Notes = updatedRental.Notes;
            rental.UpdatedAt = DateTime.Now;
            rental.Device = updatedRental.Device;
            rental.User = updatedRental.User;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRental(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if(rental == null)
            {
                return NotFound();
            }

            if(rental.Status != RentalStatus.Pending)
            {
                return BadRequest("The status must be pending to be approved");
            }

            rental.ApprovedAt = DateTime.Now;
            rental.Status = RentalStatus.Approved;
            rental.UpdatedAt = rental.UpdatedAt;

            await _context.SaveChangesAsync();
            return Ok(rental);
        }

        [HttpPost("return-rental/{id}")]
        public async Task<IActionResult> ReturnRental(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if(rental == null) {
                return NotFound();
            }

            if(rental.Status != RentalStatus.Approved)
            {
                return BadRequest("An Item with Status thats not approved cant be returned");
            }

            rental.ActualReturnDate = DateTime.Now;
            rental.Status = RentalStatus.Returned;
            rental.UpdatedAt = DateTime.Now;

            return Ok(rental);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> CancelRental(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if(rental == null)
            {
                return NotFound();
            }

            if(rental.Status != RentalStatus.Pending)
            {
                return BadRequest("Your Rental isnt pending. It cant be deleted");
            }

            _context.Rentals.Remove(rental);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    


}
