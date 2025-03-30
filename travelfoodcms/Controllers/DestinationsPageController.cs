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
    public class DestinationsPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DestinationsPageController(ApplicationDbContext context)
        {
            _context = context;
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
            return View(new DestinationViewModel());
        }

        // POST: Destinations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DestinationViewModel destinationViewModel)
        {
            if (ModelState.IsValid)
            {
                var destination = new Destination
                {
                    Name = destinationViewModel.Name,
                    Location = destinationViewModel.Location,
                    Description = destinationViewModel.Description,
                    ImageUrl = destinationViewModel.ImageUrl ?? string.Empty,
                    Date = DateTime.Now,
                    CreatorUserId = destinationViewModel.CreatorUserId
                };

                _context.Add(destination);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
        public async Task<IActionResult> Edit(int id, DestinationViewModel destinationViewModel)
        {
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

                    destination.Name = destinationViewModel.Name;
                    destination.Location = destinationViewModel.Location;
                    destination.Description = destinationViewModel.Description;
                    destination.ImageUrl = destinationViewModel.ImageUrl ?? string.Empty;

                    _context.Update(destination);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DestinationExists(destinationViewModel.DestinationId))
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