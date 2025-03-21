using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelFoodCms.Data;
using TravelFoodCms.Models;

namespace TravelFoodCms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.Restaurant)
                .Include(o => o.User)
                .ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Restaurant)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // GET: api/Orders/ByUser/5
        [HttpGet("ByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return await _context.Orders
                .Include(o => o.Restaurant)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        // GET: api/Orders/ByRestaurant/5
        [HttpGet("ByRestaurant/{restaurantId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByRestaurant(int restaurantId)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null)
            {
                return NotFound("Restaurant not found");
            }

            return await _context.Orders
                .Include(o => o.User)
                .Where(o => o.RestaurantId == restaurantId)
                .ToListAsync();
        }

        // GET: api/Orders/5/OrderItems
        [HttpGet("{id}/OrderItems")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == id)
                .ToListAsync();

            return orderItems;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            // Verify the restaurant exists
            var restaurant = await _context.Restaurants.FindAsync(order.RestaurantId);
            if (restaurant == null)
            {
                return BadRequest("Invalid Restaurant ID");
            }

            // Verify the user exists
            var user = await _context.Users.FindAsync(order.UserId);
            if (user == null)
            {
                return BadRequest("Invalid User ID");
            }

            // Set default values
            order.OrderDate = DateTime.Now;
            order.Status = order.Status ?? "pending";

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Load related data for the response
            await _context.Entry(order)
                .Reference(o => o.Restaurant)
                .LoadAsync();
            
            await _context.Entry(order)
                .Reference(o => o.User)
                .LoadAsync();

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = order.OrderId },
                order);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            // Verify the restaurant exists
            var restaurant = await _context.Restaurants.FindAsync(order.RestaurantId);
            if (restaurant == null)
            {
                return BadRequest("Invalid Restaurant ID");
            }

            // Verify the user exists
            var user = await _context.Users.FindAsync(order.UserId);
            if (user == null)
            {
                return BadRequest("Invalid User ID");
            }

            // Get the original order to preserve create date
            var originalOrder = await _context.Orders.FindAsync(id);
            if (originalOrder == null)
            {
                return NotFound();
            }

            // Preserve original order date
            order.OrderDate = originalOrder.OrderDate;
            
            _context.Entry(originalOrder).State = EntityState.Detached;
            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // PATCH: api/Orders/5/UpdateStatus
        [HttpPatch("{id}/UpdateStatus")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatusUpdate statusUpdate)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = statusUpdate.Status;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            // This will cascade delete all order items associated with this order
            // due to our DbContext configuration
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }

    // Helper class for updating order status
    public class OrderStatusUpdate
    {
        public string Status { get; set; }
    }
}