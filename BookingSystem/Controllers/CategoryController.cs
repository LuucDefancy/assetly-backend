using BookingSystem.Data;
using BookingSystem.DTOs;
using BookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly FirstNetAPIContext _context;
        public CategoryController(FirstNetAPIContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var categories = await _context.Categories
                .ToListAsync();


            return Ok(categories);
        }
    }
}
