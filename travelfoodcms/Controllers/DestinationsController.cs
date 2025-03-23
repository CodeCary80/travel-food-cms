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
    public class DestinationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DestinationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Destinations
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DestinationDTO>>> GetDestinations()
        {
             var destinations = await _context.Destinations.ToListAsync();

             var destinationDTOs = destinations.Select(d => new DestinationDTO
            {
                DestinationId = d.DestinationId,
                Name = d.Name,
                Location = d.Location,
                Description = d.Description,
                ImageUrl = d.ImageUrl,
                Date = d.Date,
                Restaurants = null 
            }).ToList();

            return destinationDTOs;
        }

        // GET: api/Destinations/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DestinationDTO>> GetDestination(int id)
        {
            var destination = await _context.Destinations.FindAsync(id);

            if (destination == null)
            {
                return NotFound();
            }

            var destinationDTO = new DestinationDTO
            {
                DestinationId = destination.DestinationId,
                Name = destination.Name,
                Location = destination.Location,
                Description = destination.Description,
                ImageUrl = destination.ImageUrl,
                Date = destination.Date,
                Restaurants = destination.Restaurants?.Select(r => new RestaurantDTO
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
                }).ToList()
            };

            return destinationDTO;
        }

        // GET: api/Destinations/5/Restaurants
        [HttpGet("{id}/Restaurants")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RestaurantDTO>>> GetDestinationRestaurants(int id)
        {
            var destination = await _context.Destinations.FindAsync(id);

            if (destination == null)
            {
                return NotFound();
            }

            var restaurants = await _context.Restaurants
                .Where(r => r.DestinationId == id)
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
                Orders = null // Not loading orders here
            }).ToList();

            return restaurantDTOs;
        }

        // POST: api/Destinations
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DestinationDTO>> CreateDestination(DestinationDTO destinationDTO)
        {
             var destination = new Destination
            {
                Name = destinationDTO.Name,
                Location = destinationDTO.Location,
                Description = destinationDTO.Description,
                ImageUrl = destinationDTO.ImageUrl,
                Date = DateTime.Now
            };

            _context.Destinations.Add(destination);
            await _context.SaveChangesAsync();

            destinationDTO.DestinationId = destination.DestinationId;
            destinationDTO.Date = destination.Date;

            return CreatedAtAction(
                nameof(GetDestination),
                new { id = destination.DestinationId },
                destinationDTO);
        }

        // PUT: api/Destinations/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDestination(int id, Destination destinationDTO)
        {
            if (id != destinationDTO.DestinationId)
            {
                return BadRequest();
            }

            var originalDestination = await _context.Destinations.FindAsync(id);
            if (originalDestination == null)
            {
                return NotFound();
            }

            originalDestination.Name = destinationDTO.Name;
            originalDestination.Location = destinationDTO.Location;
            originalDestination.Description = destinationDTO.Description;
            originalDestination.ImageUrl = destinationDTO.ImageUrl;
            
            _context.Entry(originalDestination).State = EntityState.Modified;

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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDestination(int id)
        {
            var destination = await _context.Destinations
                .Include(d => d.Restaurants)
                .FirstOrDefaultAsync(d => d.DestinationId == id);
                
            if (destination == null)
            {
                return NotFound();
            }

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