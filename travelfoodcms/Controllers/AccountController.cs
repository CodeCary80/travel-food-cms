
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelFoodCms.Data;
using TravelFoodCms.Models.ViewModels;
using TravelFoodCms.Models;
using TravelFoodCms.Models.DTOs;


public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Displays the login page
    /// </summary>
    /// <returns>Login view</returns>
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }


    /// <summary>
    /// Processes user login attempt
    /// </summary>
    /// <param name="model">Login credentials</param>
    /// <returns>Redirects to home page on success, returns to login page on failure</returns>
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        try 
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    // Log failed login attempt
                    Console.WriteLine($"Login failed: User not found - {model.Email}");
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }

                var hashedPassword = HashPassword(model.Password);

                Console.WriteLine($"Entered Email: {model.Email}");
                Console.WriteLine($"Stored Email: {user.Email}");
                Console.WriteLine($"Entered Password Hash: {hashedPassword}");
                Console.WriteLine($"Stored Password Hash: {user.PasswordHash}");
                Console.WriteLine($"Password Match: {hashedPassword == user.PasswordHash}");

                if (user.PasswordHash != hashedPassword)
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login Exception: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            ModelState.AddModelError("", "An unexpected error occurred.");
            return View(model);
        }
    }

        /// <summary>
        /// Logs out the current user
        /// </summary>
        /// <returns>Redirects to home page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try 
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Logout error: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }


        /// <summary>
        /// Displays access denied page
        /// </summary
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
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


        /// <summary>
        /// Displays the user registration page
        /// </summary>
        /// <returns>Register view</returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        /// <summary>
        /// Processes new user registration
        /// </summary>
        /// <param name="model">User registration details</param>
        /// <returns>Redirects to home page after successful registration</returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username or email already exists
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username is already taken");
                    return View(model);
                }

                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use");
                    return View(model);
                }

                // Create user with default non-admin status
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.NewPassword),
                    IsAdmin = false // Explicitly set to non-admin
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Optional: Automatically log in after registration
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "User")
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        private string DebugHashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
}