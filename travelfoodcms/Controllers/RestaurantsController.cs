using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelFoodCms.Data;
using TravelFoodCms.Models;
using TravelFoodCms.Models.DTOs;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RestaurantDTO>>> GetRestaurants()
        {
             var restaurants = await _context.Restaurants
                .Include(r => r.Destination)
                .ToListAsync();

            var restaurantDTOs = restaurants.Select(r => new RestaurantDTO
            {
                RestaurantId = r.RestaurantId,
                DestinationId = r.DestinationId,
                Name = r.Name,
                CuisineType = r.CuisineType,
                PriceRange = r.PriceRange,
                ContactInfo = r.ContactInfo,
                OperatingHours = r.OperatingHours,
                Address = r.Address,
                ImageUrl = r.ImageUrl,
                Date = r.Date,
                Orders = null 
            }).ToList();

            return restaurantDTOs;
        }

        // GET: api/Restaurants/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RestaurantDTO>> GetRestaurant(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Destination)
                .FirstOrDefaultAsync(r => r.RestaurantId == id);

            if (restaurant == null)
            {
                return NotFound();
            }

             var restaurantDTO = new RestaurantDTO
            {
                RestaurantId = restaurant.RestaurantId,
                DestinationId = restaurant.DestinationId,
                Name = restaurant.Name,
                CuisineType = restaurant.CuisineType,
                PriceRange = restaurant.PriceRange,
                ContactInfo = restaurant.ContactInfo,
                OperatingHours = restaurant.OperatingHours,
                Address = restaurant.Address,
                ImageUrl = restaurant.ImageUrl,
                Date = restaurant.Date,
                Orders = restaurant.Orders?.Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    RestaurantId = o.RestaurantId,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    SpecialRequests = o.SpecialRequests,
                    OrderItems = null 
                }).ToList()
            };

            return restaurantDTO;
        }

        // GET: api/Restaurants/5/Orders
        [HttpGet("{id}/Orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetRestaurantOrders(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Where(o => o.RestaurantId == id)
                .ToListAsync();

            var orderDTOs = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                RestaurantId = o.RestaurantId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                SpecialRequests = o.SpecialRequests,
                OrderItems = null 
            }).ToList();

            return orderDTOs;
        }

        // POST: api/Restaurants
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RestaurantDTO>> CreateRestaurant(RestaurantDTO restaurantDTO)
        {
            // Verify the destination exists
            var destination = await _context.Destinations.FindAsync(restaurantDTO.DestinationId);
            if (destination == null)
            {
                return BadRequest("Invalid Destination ID");
            }

             var restaurant = new Restaurant
            {
                DestinationId = restaurantDTO.DestinationId,
                Name = restaurantDTO.Name,
                CuisineType = restaurantDTO.CuisineType,
                PriceRange = restaurantDTO.PriceRange,
                ContactInfo = restaurantDTO.ContactInfo,
                OperatingHours = restaurantDTO.OperatingHours,
                Address = restaurantDTO.Address,
                ImageUrl = restaurantDTO.ImageUrl,
                Date = DateTime.Now
            };

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            // Update the DTO with the newly created ID and date
            restaurantDTO.RestaurantId = restaurant.RestaurantId;
            restaurantDTO.Date = restaurant.Date;

            return CreatedAtAction(
                nameof(GetRestaurant),
                new { id = restaurant.RestaurantId },
                restaurantDTO);
        }

        // PUT: api/Restaurants/5
        [HttpPut("{id}")]
         [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRestaurant(int id, RestaurantDTO restaurantDTO)
        {
            if (id != restaurantDTO.RestaurantId)
            {
                return BadRequest();
            }

            // Verify the destination exists
            var destination = await _context.Destinations.FindAsync(restaurantDTO.DestinationId);
            if (destination == null)
            {
                return BadRequest("Invalid Destination ID");
            }

            var originalRestaurant = await _context.Restaurants.FindAsync(id);
            if (originalRestaurant == null)
            {
                return NotFound();
            }

            // Update restaurant properties
            originalRestaurant.DestinationId = restaurantDTO.DestinationId;
            originalRestaurant.Name = restaurantDTO.Name;
            originalRestaurant.CuisineType = restaurantDTO.CuisineType;
            originalRestaurant.PriceRange = restaurantDTO.PriceRange;
            originalRestaurant.ContactInfo = restaurantDTO.ContactInfo;
            originalRestaurant.OperatingHours = restaurantDTO.OperatingHours;
            originalRestaurant.Address = restaurantDTO.Address;
            originalRestaurant.ImageUrl = restaurantDTO.ImageUrl;

            _context.Entry(originalRestaurant).State = EntityState.Modified;


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
         [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Orders)
                .FirstOrDefaultAsync(r => r.RestaurantId == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Restaurants/ByDestination/5
        [HttpGet("ByDestination/{destinationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RestaurantDTO>>> GetRestaurantsByDestination(int destinationId)
        {
            var destination = await _context.Destinations.FindAsync(destinationId);
            if (destination == null)
            {
                return NotFound("Destination not found");
            }

            var restaurants = await _context.Restaurants
                .Where(r => r.DestinationId == destinationId)
                .ToListAsync();

             var restaurantDTOs = restaurants.Select(r => new RestaurantDTO
            {
                RestaurantId = r.RestaurantId,
                DestinationId = r.DestinationId,
                Name = r.Name,
                CuisineType = r.CuisineType,
                PriceRange = r.PriceRange,
                ContactInfo = r.ContactInfo,
                OperatingHours = r.OperatingHours,
                Address = r.Address,
                ImageUrl = r.ImageUrl,
                Date = r.Date,
                Orders = null 
            }).ToList();

            return restaurantDTOs;
        }

        private bool RestaurantExists(int id)
        {
            return _context.Restaurants.Any(e => e.RestaurantId == id);
        }
    }
}