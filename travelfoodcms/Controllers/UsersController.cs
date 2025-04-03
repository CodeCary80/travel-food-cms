using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelFoodCms.Data;
using TravelFoodCms.Models;
using System.Security.Cryptography;
using System.Text;
using TravelFoodCms.Models.DTOs;

namespace TravelFoodCms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        /// curl -X "GET" https://localhost:5234/api/Users
        /// <summary>
        /// Returns a list of all Users
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{User},{User},...]
        /// </returns>
        /// <example>
        /// GET: api/Users -> [{User},{User},...]
        /// </example>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            // For security reasons, don't include password hashes in the response
            return await _context.Users
                .Select(u => new User
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    IsAdmin = u.IsAdmin
                    // Omit PasswordHash
                })
                .ToListAsync();
        }

        // GET: api/Users/5
        /// curl -X "GET" https://localhost:5234/api/Users/5
        /// <summary>
        /// Returns a single User specified by its {id}
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns>
        /// 200 OK
        /// {User}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Users/5 -> {User}
        /// </example>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Don't expose password hash
            user.PasswordHash = null;

            return user;
        }

        // GET: api/Users/5/Orders
        /// curl -X "GET" https://localhost:5234/api/Users/5/Orders
        /// <summary>
        /// Returns all Orders associated with a specific User
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns>
        /// 200 OK
        /// [{Order},{Order},...]
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Users/5/Orders -> [{Order},{Order},...]
        /// </example>
        [HttpGet("{id}/Orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == id)
                .Include(o => o.Restaurant)
                .ToListAsync();

            return orders;
        }

        // POST: api/Users
        /// curl -X "POST" https://localhost:5234/api/Users -H "Content-Type: application/json" -d '{"Username":"johndoe","Email":"john@example.com","PasswordHash":"password123","IsAdmin":false}'
        /// <summary>
        /// Creates a new User
        /// </summary>
        /// <param name="userDTO">The user details</param>
        /// <returns>
        /// 201 Created
        /// {UserDTO}
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// POST: api/Users -> {UserDTO}
        /// </example>
        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser(UserDTO userDTO)
        {
            // Check if username or email already exists
            if (await _context.Users.AnyAsync(u => u.Username == userDTO.Username))
            {
                return BadRequest("Username already exists");
            }

            if (await _context.Users.AnyAsync(u => u.Email == userDTO.Email))
            {
                return BadRequest("Email already exists");
            }

            var user = new User
                {
                    Username = userDTO.Username,
                    Email = userDTO.Email,
                    PasswordHash = HashPassword(userDTO.PasswordHash),
                    IsAdmin = userDTO.IsAdmin
                };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

             userDTO.PasswordHash = null;
            userDTO.UserId = user.UserId;

            return CreatedAtAction(
                nameof(GetUser),
                new { id = user.UserId },
                userDTO);
        }

        // PUT: api/Users/5
        /// curl -X "PUT" https://localhost:5234/api/Users/5 -H "Content-Type: application/json" -d '{"UserId":5,"Username":"johndoe","Email":"john@example.com","IsAdmin":true}'
        /// <summary>
        /// Updates an existing User
        /// </summary>
        /// <param name="id">The user id</param>
        /// <param name="userDTO">The updated user details</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// PUT: api/Users/5 -> 204 No Content
        /// </example>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDTO userDTO)
        {
            if (id != userDTO.UserId)
            {
                return BadRequest();
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Check if username is changing and if it's already taken
            if (existingUser.Username != userDTO.Username && 
                await _context.Users.AnyAsync(u => u.Username == userDTO.Username))
            {
                return BadRequest("Username already exists");
            }

            // Check if email is changing and if it's already taken
            if (existingUser.Email != userDTO.Email && 
                await _context.Users.AnyAsync(u => u.Email == userDTO.Email))
            {
                return BadRequest("Email already exists");
            }

            existingUser.Username = userDTO.Username;
            existingUser.Email = userDTO.Email;
            existingUser.IsAdmin = userDTO.IsAdmin;

            if (!string.IsNullOrEmpty(userDTO.PasswordHash))
            {
                existingUser.PasswordHash = HashPassword(userDTO.PasswordHash);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // DELETE: api/Users/5
        /// curl -X "DELETE" https://localhost:5234/api/Users/5
        /// <summary>
        /// Deletes a User
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Users/5 -> 204 No Content
        /// </example>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            // This will cascade delete all orders associated with this user
            // due to our DbContext configuration
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Users/Authenticate
        //didn't use in MVP stage at the moment. it will be added in the next stage.
        [HttpPost("Authenticate")]
        public async Task<ActionResult<User>> Authenticate(LoginModel login)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == login.Username);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            // Check password
            if (user.PasswordHash != HashPassword(login.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            // Don't expose password hash
            var authenticatedUser = new User
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsAdmin = user.IsAdmin
            };

            return authenticatedUser;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        private string HashPassword(string password)
        {
            // NOTE: In a real application, use a proper password hashing library like BCrypt
            // This is a simple hash for demonstration purposes only
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }

    // Helper class for login
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}