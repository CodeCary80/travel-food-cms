using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelFoodCms.Data;
using TravelFoodCms.Models;
using TravelFoodCms.Models.ViewModels;

namespace TravelFoodCms.Controllers
{
    public class RestaurantsPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestaurantsPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RestaurantsPage
        public async Task<IActionResult> Index()
        {
            var restaurants = await _context.Restaurants
                .Include(r => r.Destination)
                .Include(r => r.Orders)
                .ToListAsync();

            var restaurantViewModels = restaurants.Select(r => new RestaurantViewModel
            {
                RestaurantId = r.RestaurantId,
                Name = r.Name,
                DestinationId = r.DestinationId,
                DestinationName = r.Destination?.Name,
                CuisineType = r.CuisineType,
                PriceRange = r.PriceRange,
                Address = r.Address,
                TotalOrders = r.Orders?.Count ?? 0
            }).ToList();

            return View(restaurantViewModels);
        }

        // GET: RestaurantsPage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .Include(r => r.Destination)
                .Include(r => r.Orders)
                .FirstOrDefaultAsync(m => m.RestaurantId == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            var restaurantViewModel = new RestaurantViewModel
            {
                RestaurantId = restaurant.RestaurantId,
                Name = restaurant.Name,
                DestinationId = restaurant.DestinationId,
                DestinationName = restaurant.Destination?.Name,
                CuisineType = restaurant.CuisineType,
                PriceRange = restaurant.PriceRange,
                ContactInfo = restaurant.ContactInfo,
                OperatingHours = restaurant.OperatingHours,
                Address = restaurant.Address,
                ImageUrl = restaurant.ImageUrl,
                Date = restaurant.Date,
                TotalOrders = restaurant.Orders?.Count ?? 0,
                Orders = restaurant.Orders?.Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                }).ToList()
            };

            return View(restaurantViewModel);
        }

        // GET: RestaurantsPage/Create
        public IActionResult Create()
        {
            // Populate destination dropdown
            ViewBag.Destinations = _context.Destinations
                .Select(d => new { d.DestinationId, d.Name })
                .ToList();

            return View(new RestaurantViewModel());
        }

        // POST: RestaurantsPage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RestaurantViewModel restaurantViewModel)
        {
            if (ModelState.IsValid)
            {
                var restaurant = new Restaurant
                {
                    Name = restaurantViewModel.Name,
                    DestinationId = restaurantViewModel.DestinationId,
                    CuisineType = restaurantViewModel.CuisineType,
                    PriceRange = restaurantViewModel.PriceRange,
                    ContactInfo = restaurantViewModel.ContactInfo,
                    OperatingHours = restaurantViewModel.OperatingHours,
                    Address = restaurantViewModel.Address,
                    ImageUrl = restaurantViewModel.ImageUrl ?? string.Empty,
                    Date = DateTime.Now
                };

                _context.Add(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate destination dropdown if model is invalid
            ViewBag.Destinations = _context.Destinations
                .Select(d => new { d.DestinationId, d.Name })
                .ToList();

            return View(restaurantViewModel);
        }

        // GET: RestaurantsPage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            // Populate destination dropdown
            ViewBag.Destinations = _context.Destinations
                .Select(d => new { d.DestinationId, d.Name })
                .ToList();

            var restaurantViewModel = new RestaurantViewModel
            {
                RestaurantId = restaurant.RestaurantId,
                Name = restaurant.Name,
                DestinationId = restaurant.DestinationId,
                CuisineType = restaurant.CuisineType,
                PriceRange = restaurant.PriceRange,
                ContactInfo = restaurant.ContactInfo,
                OperatingHours = restaurant.OperatingHours,
                Address = restaurant.Address,
                ImageUrl = restaurant.ImageUrl
            };

            return View(restaurantViewModel);
        }

        // POST: RestaurantsPage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RestaurantViewModel restaurantViewModel)
        {
            if (id != restaurantViewModel.RestaurantId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var restaurant = await _context.Restaurants.FindAsync(id);
                    if (restaurant == null)
                    {
                        return NotFound();
                    }

                    restaurant.Name = restaurantViewModel.Name;
                    restaurant.DestinationId = restaurantViewModel.DestinationId;
                    restaurant.CuisineType = restaurantViewModel.CuisineType;
                    restaurant.PriceRange = restaurantViewModel.PriceRange;
                    restaurant.ContactInfo = restaurantViewModel.ContactInfo;
                    restaurant.OperatingHours = restaurantViewModel.OperatingHours;
                    restaurant.Address = restaurantViewModel.Address;
                    restaurant.ImageUrl = restaurantViewModel.ImageUrl ?? string.Empty;

                    _context.Update(restaurant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(restaurantViewModel.RestaurantId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Repopulate destination dropdown if model is invalid
            ViewBag.Destinations = _context.Destinations
                .Select(d => new { d.DestinationId, d.Name })
                .ToList();

            return View(restaurantViewModel);
        }

        // GET: RestaurantsPage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .Include(r => r.Destination)
                .FirstOrDefaultAsync(m => m.RestaurantId == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            var restaurantViewModel = new RestaurantViewModel
            {
                RestaurantId = restaurant.RestaurantId,
                Name = restaurant.Name,
                DestinationName = restaurant.Destination?.Name,
                CuisineType = restaurant.CuisineType,
                Address = restaurant.Address
            };

            return View(restaurantViewModel);
        }

        // POST: RestaurantsPage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Orders)
                .FirstOrDefaultAsync(m => m.RestaurantId == id);
            
            if (restaurant == null)
            {
                return NotFound();
            }

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(int id)
        {
            return _context.Restaurants.Any(e => e.RestaurantId == id);
        }
    }
}