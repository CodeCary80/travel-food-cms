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
        /// curl -X "GET" https://localhost:5234/api/Destinations
        /// <summary>
        /// Returns a list of all Destinations
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{DestinationDTO},{DestinationDTO},...]
        /// </returns>
        /// <example>
        /// GET: api/Destinations -> [{DestinationDTO},{DestinationDTO},...]
        /// </example>
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
        /// curl -X "GET" https://localhost:5234/api/Destinations/5
        /// <summary>
        /// Returns a single Destination specified by its {id}
        /// </summary>
        /// <param name="id">The destination id</param>
        /// <returns>
        /// 200 OK
        /// {DestinationDTO}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Destinations/5 -> {DestinationDTO}
        /// </example>
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
                }).ToList() ?? new List<RestaurantDTO>()
            };

            return destinationDTO;
        }

        // GET: api/Destinations/5/Restaurants
        /// curl -X "GET" https://localhost:5234/api/Destinations/5/Restaurants
        /// <summary>
        /// Returns all Restaurants associated with a specific Destination
        /// </summary>
        /// <param name="id">The destination id</param>
        /// <returns>
        /// 200 OK
        /// [{RestaurantDTO},{RestaurantDTO},...]
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Destinations/5/Restaurants -> [{RestaurantDTO},{RestaurantDTO},...]
        /// </example>
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
        /// curl -X "POST" https://localhost:5234/api/Destinations -H "Content-Type: application/json" -d '{"Name":"Paris","Location":"France","Description":"City of Lights"}'
        /// <summary>
        /// Creates a new Destination
        /// </summary>
        /// <param name="destinationDTO">The destination details</param>
        /// <returns>
        /// 201 Created
        /// {DestinationDTO}
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// POST: api/Destinations -> {DestinationDTO}
        /// </example>
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
        /// curl -X "PUT" https://localhost:5234/api/Destinations/5 -H "Content-Type: application/json" -d '{"DestinationId":5,"Name":"Paris","Location":"France","Description":"Updated description"}'
        /// <summary>
        /// Updates an existing Destination
        /// </summary>
        /// <param name="id">The destination id</param>
        /// <param name="destinationDTO">The updated destination details</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// PUT: api/Destinations/5 -> 204 No Content
        /// </example>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDestination(int id, DestinationDTO destinationDTO)
        {
            if (id != destinationDTO.DestinationId)
            {
                return BadRequest();
            }

            var originalDestination = await _context.Destinations
                .Include(d => d.Restaurants) 
                .FirstOrDefaultAsync(d => d.DestinationId == id);


            if (originalDestination == null)
            {
                return NotFound();
            }

            // Only update CreatorUserId if it's a valid user
            if (destinationDTO.CreatorUserId > 0)
            {
                var userExists = await _context.Users.AnyAsync(u => u.UserId == destinationDTO.CreatorUserId);
                if (!userExists)
                {
                    return BadRequest("Invalid User ID");
                }
                originalDestination.CreatorUserId = destinationDTO.CreatorUserId;
            }

            originalDestination.Name = destinationDTO.Name;
            originalDestination.Location = destinationDTO.Location;
            originalDestination.Description = destinationDTO.Description;
            originalDestination.ImageUrl = destinationDTO.ImageUrl;

            if (originalDestination.Restaurants == null)
            {
                originalDestination.Restaurants = new List<Restaurant>();
            }
            
            
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
        /// curl -X "DELETE" https://localhost:5234/api/Destinations/5
        /// <summary>
        /// Deletes a Destination
        /// </summary>
        /// <param name="id">The destination id</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Destinations/5 -> 204 No Content
        /// </example>
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