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
    public class RestaurantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RestaurantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Restaurants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
            return await _context.Restaurants
                .Include(r => r.Destination)
                .ToListAsync();
        }

        // GET: api/Restaurants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Destination)
                .FirstOrDefaultAsync(r => r.RestaurantId == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return restaurant;
        }

        // GET: api/Restaurants/5/Orders
        [HttpGet("{id}/Orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetRestaurantOrders(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Where(o => o.RestaurantId == id)
                .ToListAsync();

            return orders;
        }

        // POST: api/Restaurants
        [HttpPost]
        public async Task<ActionResult<Restaurant>> CreateRestaurant(Restaurant restaurant)
        {
            // Verify the destination exists
            var destination = await _context.Destinations.FindAsync(restaurant.DestinationId);
            if (destination == null)
            {
                return BadRequest("Invalid Destination ID");
            }

            restaurant.Date = DateTime.Now;
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            // Load the destination data for the response
            await _context.Entry(restaurant)
                .Reference(r => r.Destination)
                .LoadAsync();

            return CreatedAtAction(
                nameof(GetRestaurant),
                new { id = restaurant.RestaurantId },
                restaurant);
        }

        // PUT: api/Restaurants/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(int id, Restaurant restaurant)
        {
            if (id != restaurant.RestaurantId)
            {
                return BadRequest();
            }

            // Verify the destination exists
            var destination = await _context.Destinations.FindAsync(restaurant.DestinationId);
            if (destination == null)
            {
                return BadRequest("Invalid Destination ID");
            }

            // Preserve the original creation date
            var originalRestaurant = await _context.Restaurants.FindAsync(id);
            if (originalRestaurant == null)
            {
                return NotFound();
            }

            // Update restaurant and maintain original Date
            restaurant.Date = originalRestaurant.Date;
            _context.Entry(originalRestaurant).State = EntityState.Detached;

            _context.Entry(restaurant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantExists(id))
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

        // DELETE: api/Restaurants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Orders)
                .FirstOrDefaultAsync(r => r.RestaurantId == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            // This will cascade delete all orders associated with this restaurant
            // due to our DbContext configuration
            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Restaurants/ByDestination/5
        [HttpGet("ByDestination/{destinationId}")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurantsByDestination(int destinationId)
        {
            var destination = await _context.Destinations.FindAsync(destinationId);
            if (destination == null)
            {
                return NotFound("Destination not found");
            }

            return await _context.Restaurants
                .Where(r => r.DestinationId == destinationId)
                .ToListAsync();
        }

        private bool RestaurantExists(int id)
        {
            return _context.Restaurants.Any(e => e.RestaurantId == id);
        }
    }
}