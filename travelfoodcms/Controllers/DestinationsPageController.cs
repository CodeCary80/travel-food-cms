using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelFoodCms.Data;
using TravelFoodCms.Models;
using TravelFoodCms.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TravelFoodCms.Controllers
{
    public class DestinationsPageController : Controller
    {
        private readonly ApplicationDbContext _context;

         private readonly IWebHostEnvironment _hostingEnvironment;

        public DestinationsPageController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }


        // GET: Destinations
        public async Task<IActionResult> Index()
        {
            var destinations = await _context.Destinations
                .Include(d => d.Restaurants)
                .ToListAsync();

            var destinationViewModels = destinations.Select(d => new DestinationViewModel
                {
                    DestinationId = d.DestinationId,
                    Name = d.Name,
                    Location = d.Location,
                    Description = d.Description,
                    ImageUrl = d.ImageUrl,
                    Date = d.Date,
                    RestaurantCount = d.Restaurants?.Count ?? 0
                }).ToList();

            return View(destinationViewModels);
        }

        // GET: Destinations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var destination = await _context.Destinations
                .Include(d => d.Restaurants)
                .FirstOrDefaultAsync(m => m.DestinationId == id);

            if (destination == null)
            {
                return NotFound();
            }

            var destinationViewModel = new DestinationViewModel
            {
                DestinationId = destination.DestinationId,
                Name = destination.Name,
                Location = destination.Location,
                Description = destination.Description,
                ImageUrl = destination.ImageUrl,
                Date = destination.Date,
                CreatorUserId = destination.CreatorUserId,
                RestaurantCount = destination.Restaurants?.Count ?? 0,
                Restaurants = destination.Restaurants?.Select(r => new RestaurantViewModel
                {
                    RestaurantId = r.RestaurantId,
                    Name = r.Name,
                    CuisineType = r.CuisineType,
                    Address = r.Address
                }).ToList()
            };

            return View(destinationViewModel);
        }

        // GET: Destinations/Create
        public IActionResult Create()
        {
            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem 
                { 
                    Value = u.UserId.ToString(), 
                    Text = u.Username 
                })
                .ToList();

            return View(new DestinationViewModel());
            
        }

        // POST: Destinations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DestinationViewModel destinationViewModel, IFormFile ImageFile)
        {
            // Remove Restaurants validation
            ModelState.Remove("Restaurants");
            // Remove ImageUrl validation
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
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

                var destination = new Destination
                {
                    Name = destinationViewModel.Name,
                    Location = destinationViewModel.Location,
                    Description = destinationViewModel.Description,
                    ImageUrl = uniqueFileName != null ? "/images/" + uniqueFileName : null,
                    Date = DateTime.Now,
                    CreatorUserId = destinationViewModel.CreatorUserId
                };

                _context.Add(destination);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem 
                { 
                    Value = u.UserId.ToString(), 
                    Text = u.Username 
                })
                .ToList();

            return View(destinationViewModel);
        }


                // GET: Destinations/Edit/5
                public async Task<IActionResult> Edit(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var destination = await _context.Destinations.FindAsync(id);
                    if (destination == null)
                    {
                        return NotFound();
                    }

                    ViewBag.Users = _context.Users
                            .Select(u => new SelectListItem 
                            { 
                                Value = u.UserId.ToString(), 
                                Text = u.Username 
                            })
                            .ToList();

                    var destinationViewModel = new DestinationViewModel
                    {
                        DestinationId = destination.DestinationId,
                        Name = destination.Name,
                        Location = destination.Location,
                        Description = destination.Description,
                        ImageUrl = destination.ImageUrl,
                        CreatorUserId = destination.CreatorUserId
                    };

                    return View(destinationViewModel);
                }

        // POST: Destinations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DestinationViewModel destinationViewModel, IFormFile ImageFile)
        {
            // Remove Restaurants validation
            ModelState.Remove("Restaurants");
            // Remove ImageUrl validation
            ModelState.Remove("ImageUrl");

            if (id != destinationViewModel.DestinationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var destination = await _context.Destinations.FindAsync(id);
                    if (destination == null)
                    {
                        return NotFound();
                    }

                    // Handle image upload
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                        
                        // Create directory if it doesn't exist
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
                        
                        // Update ImageUrl
                        destination.ImageUrl = "/images/" + uniqueFileName;
                    }

                    // Update other properties
                    destination.Name = destinationViewModel.Name;
                    destination.Location = destinationViewModel.Location;
                    destination.Description = destinationViewModel.Description;
                    destination.CreatorUserId = destinationViewModel.CreatorUserId;

                    _context.Update(destination);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception
                    ViewBag.Users = _context.Users
                        .Select(u => new SelectListItem 
                        { 
                            Value = u.UserId.ToString(), 
                            Text = u.Username,
                            Selected = u.UserId == destinationViewModel.CreatorUserId
                        })
                        .ToList();

                    ModelState.AddModelError("", "An error occurred while saving the destination: " + ex.Message);
                    return View(destinationViewModel);
                }
            }

            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem 
                { 
                    Value = u.UserId.ToString(), 
                    Text = u.Username,
                    Selected = u.UserId == destinationViewModel.CreatorUserId
                })
                .ToList();

            return View(destinationViewModel);
        }

                // GET: Destinations/Delete/5
                public async Task<IActionResult> Delete(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var destination = await _context.Destinations
                        .FirstOrDefaultAsync(m => m.DestinationId == id);
                    if (destination == null)
                    {
                        return NotFound();
                    }

                    var destinationViewModel = new DestinationViewModel
                    {
                        DestinationId = destination.DestinationId,
                        Name = destination.Name,
                        Location = destination.Location,
                        Description = destination.Description,
                        ImageUrl = destination.ImageUrl
                    };

                    return View(destinationViewModel);
                }

        // POST: Destinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var destination = await _context.Destinations
                .Include(d => d.Restaurants)
                .FirstOrDefaultAsync(m => m.DestinationId == id);
            
            if (destination == null)
            {
                return NotFound();
            }

            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DestinationExists(int id)
        {
            return _context.Destinations.Any(e => e.DestinationId == id);
        }
    }
}