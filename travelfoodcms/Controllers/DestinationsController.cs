using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelFoodCms.Data;
using TravelFoodCms.Models;

namespace TravelFoodCms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DestinationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Destinations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Destination>>> GetDestinations()
        {
            return await _context.Destinations.ToListAsync();
        }

        // GET: api/Destinations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Destination>> GetDestination(int id)
        {
            var destination = await _context.Destinations.FindAsync(id);

            if (destination == null)
            {
                return NotFound();
            }

            return destination;
        }

        // GET: api/Destinations/5/Restaurants
        [HttpGet("{id}/Restaurants")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetDestinationRestaurants(int id)
        {
            var destination = await _context.Destinations.FindAsync(id);

            if (destination == null)
            {
                return NotFound();
            }

            var restaurants = await _context.Restaurants
                .Where(r => r.DestinationId == id)
                .ToListAsync();

            return restaurants;
        }

        // POST: api/Destinations
        [HttpPost]
        public async Task<ActionResult<Destination>> CreateDestination(Destination destination)
        {
            destination.Date = DateTime.Now;
            _context.Destinations.Add(destination);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDestination), 
                new { id = destination.DestinationId }, 
                destination);
        }

        // PUT: api/Destinations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDestination(int id, Destination destination)
        {
            if (id != destination.DestinationId)
            {
                return BadRequest();
            }

            // Preserve the original creation date
            var originalDestination = await _context.Destinations.FindAsync(id);
            if (originalDestination == null)
            {
                return NotFound();
            }

            // Update destination and maintain original Date
            destination.Date = originalDestination.Date;
            _context.Entry(originalDestination).State = EntityState.Detached;
            
            _context.Entry(destination).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DestinationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Destinations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDestination(int id)
        {
            var destination = await _context.Destinations
                .Include(d => d.Restaurants)
                .FirstOrDefaultAsync(d => d.DestinationId == id);
                
            if (destination == null)
            {
                return NotFound();
            }

            // This will cascade delete all restaurants associated with this destination
            // due to our DbContext configuration
            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DestinationExists(int id)
        {
            return _context.Destinations.Any(e => e.DestinationId == id);
        }
    }
}