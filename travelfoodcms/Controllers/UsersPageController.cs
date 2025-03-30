using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelFoodCms.Data;
using TravelFoodCms.Models;
using TravelFoodCms.Models.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace TravelFoodCms.Controllers
{
    public class UsersPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UsersPage
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.Orders)
                .ToListAsync();

            var userViewModels = users.Select(u => new UserViewModel
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                IsAdmin = u.IsAdmin,
                TotalOrderCount = u.Orders?.Count ?? 0,
                TotalSpending = u.Orders?.Sum(o => o.TotalAmount) ?? 0
            }).ToList();

            return View(userViewModels);
        }

        // GET: UsersPage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Orders)
                    .ThenInclude(o => o.Restaurant)
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                TotalOrderCount = user.Orders?.Count ?? 0,
                TotalSpending = user.Orders?.Sum(o => o.TotalAmount) ?? 0,
                Orders = user.Orders?.Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    RestaurantName = o.Restaurant?.Name,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                }).ToList()
            };

            return View(userViewModel);
        }

        // GET: UsersPage/Create
        public IActionResult Create()
        {
            return View(new RegisterViewModel());
        }

        // POST: UsersPage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if username or email already exists
                if (await _context.Users.AnyAsync(u => u.Username == registerViewModel.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    return View(registerViewModel);
                }

                if (await _context.Users.AnyAsync(u => u.Email == registerViewModel.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(registerViewModel);
                }

                var user = new User
                {
                    Username = registerViewModel.Username,
                    Email = registerViewModel.Email,
                    PasswordHash = HashPassword(registerViewModel.NewPassword),
                    IsAdmin = registerViewModel.IsAdmin
                };

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(registerViewModel);
        }

        // GET: UsersPage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsAdmin = user.IsAdmin
            };

            return View(userViewModel);
        }

        // POST: UsersPage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserViewModel userViewModel)
        {
            if (id != userViewModel.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users.FindAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Check if username is changing and if it's already taken
                    if (user.Username != userViewModel.Username && 
                        await _context.Users.AnyAsync(u => u.Username == userViewModel.Username))
                    {
                        ModelState.AddModelError("Username", "Username already exists");
                        return View(userViewModel);
                    }

                    // Check if email is changing and if it's already taken
                    if (user.Email != userViewModel.Email && 
                        await _context.Users.AnyAsync(u => u.Email == userViewModel.Email))
                    {
                        ModelState.AddModelError("Email", "Email already exists");
                        return View(userViewModel);
                    }

                    user.Username = userViewModel.Username;
                    user.Email = userViewModel.Email;
                    user.IsAdmin = userViewModel.IsAdmin;

                    // Only update password if a new one is provided
                    if (!string.IsNullOrEmpty(userViewModel.NewPassword))
                    {
                        user.PasswordHash = HashPassword(userViewModel.NewPassword);
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userViewModel.UserId))
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
            return View(userViewModel);
        }

        // GET: UsersPage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email
            };

            return View(userViewModel);
        }

        // POST: UsersPage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(m => m.UserId == id);
            
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Utility method for password hashing
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}