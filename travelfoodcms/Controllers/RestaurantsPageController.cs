using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TravelFoodCms.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using TravelFoodCms.Models;
using TravelFoodCms.Models.ViewModels;

namespace TravelFoodCms.Controllers
{
    [Authorize]
    public class RestaurantsPageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public RestaurantsPageController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
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
                ImageUrl = r.ImageUrl, // Add this line
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
                    .Select(d => new SelectListItem 
                    { 
                        Value = d.DestinationId.ToString(), 
                        Text = d.Name 
                    })
                    .ToList();

            return View(new RestaurantViewModel());
        }

        // POST: RestaurantsPage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RestaurantViewModel restaurantViewModel, IFormFile? ImageFile)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("DestinationName");
            ModelState.Remove("TotalOrders");
            ModelState.Remove("Orders");

            if (ModelState.IsValid)
            {
                try
                {
                    string? uniqueFileName = null;
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                        
                        // Create directory if it doesn't exist
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    var restaurant = new Restaurant
                    {
                        Name = restaurantViewModel.Name,
                        DestinationId = restaurantViewModel.DestinationId,
                        CuisineType = restaurantViewModel.CuisineType,
                        PriceRange = restaurantViewModel.PriceRange,
                        ContactInfo = restaurantViewModel.ContactInfo ?? string.Empty,
                        OperatingHours = restaurantViewModel.OperatingHours ?? string.Empty,
                        Address = restaurantViewModel.Address,
                        ImageUrl = uniqueFileName != null ? "/images/" + uniqueFileName : null,
                        Date = DateTime.Now
                    };

                    _context.Add(restaurant);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating restaurant: " + ex.Message);
                }
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            foreach (var error in errors)
            {
                Console.WriteLine("Validation Error: " + error);
            }

            // Repopulate destination dropdown
            ViewBag.Destinations = _context.Destinations
                .Select(d => new SelectListItem 
                { 
                    Value = d.DestinationId.ToString(), 
                    Text = d.Name 
                })
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
            .Select(d => new SelectListItem 
                { 
                    Value = d.DestinationId.ToString(), 
                    Text = d.Name 
                })
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, RestaurantViewModel restaurantViewModel, IFormFile? ImageFile)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("DestinationName");
            ModelState.Remove("TotalOrders");
            ModelState.Remove("Orders");

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

                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                        
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }
                        
                        restaurant.ImageUrl = uniqueFileName != null ? "/images/" + uniqueFileName : null;
                    }

                    restaurant.Name = restaurantViewModel.Name;
                    restaurant.DestinationId = restaurantViewModel.DestinationId;
                    restaurant.CuisineType = restaurantViewModel.CuisineType;
                    restaurant.PriceRange = restaurantViewModel.PriceRange;
                    restaurant.ContactInfo = restaurantViewModel.ContactInfo ?? string.Empty;
                    restaurant.OperatingHours = restaurantViewModel.OperatingHours ?? string.Empty;
                    restaurant.Address = restaurantViewModel.Address;

                    _context.Update(restaurant);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating restaurant: " + ex.Message);
                }
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            foreach (var error in errors)
            {
                Console.WriteLine("Validation Error: " + error);
            }

            // Repopulate destinations dropdown
            ViewBag.Destinations = _context.Destinations
                .Select(d => new SelectListItem 
                { 
                    Value = d.DestinationId.ToString(), 
                    Text = d.Name,
                    Selected = d.DestinationId == restaurantViewModel.DestinationId
                })
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
        [Authorize(Roles = "Admin")]
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