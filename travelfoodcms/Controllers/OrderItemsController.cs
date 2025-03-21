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
    public class OrderItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .ToListAsync();
        }

        // GET: api/OrderItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.ItemId == id);

            if (orderItem == null)
            {
                return NotFound();
            }

            return orderItem;
        }

        // GET: api/OrderItems/ByOrder/5
        [HttpGet("ByOrder/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItemsByOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }

        // POST: api/OrderItems
        [HttpPost]
        public async Task<ActionResult<OrderItem>> CreateOrderItem(OrderItem orderItem)
        {
            // Verify the order exists
            var order = await _context.Orders.FindAsync(orderItem.OrderId);
            if (order == null)
            {
                return BadRequest("Invalid Order ID");
            }

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            // Update the order's total amount
            await UpdateOrderTotal(orderItem.OrderId);

            // Load related data for the response
            await _context.Entry(orderItem)
                .Reference(oi => oi.Order)
                .LoadAsync();

            return CreatedAtAction(
                nameof(GetOrderItem),
                new { id = orderItem.ItemId },
                orderItem);
        }

        // PUT: api/OrderItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(int id, OrderItem orderItem)
        {
            if (id != orderItem.ItemId)
            {
                return BadRequest();
            }

            // Verify the order exists
            var order = await _context.Orders.FindAsync(orderItem.OrderId);
            if (order == null)
            {
                return BadRequest("Invalid Order ID");
            }

            _context.Entry(orderItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                
                // Update the order's total amount
                await UpdateOrderTotal(orderItem.OrderId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderItemExists(id))
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

        // DELETE: api/OrderItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            int orderId = orderItem.OrderId;
            
            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
            
            // Update the order's total amount
            await UpdateOrderTotal(orderId);

            return NoContent();
        }

        // POST: api/OrderItems/AddMultiple
        [HttpPost("AddMultiple")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> AddMultipleOrderItems(
            [FromBody] MultipleOrderItemsRequest request)
        {
            // Verify the order exists
            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null)
            {
                return BadRequest("Invalid Order ID");
            }

            // Set the OrderId for each item
            foreach (var item in request.OrderItems)
            {
                item.OrderId = request.OrderId;
                _context.OrderItems.Add(item);
            }

            await _context.SaveChangesAsync();
            
            // Update the order's total amount
            await UpdateOrderTotal(request.OrderId);

            // Return the created items
            var createdItems = await _context.OrderItems
                .Where(oi => oi.OrderId == request.OrderId)
                .ToListAsync();

            return CreatedAtAction(
                nameof(GetOrderItemsByOrder),
                new { orderId = request.OrderId },
                createdItems);
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.ItemId == id);
        }

        // Helper method to update an order's total amount based on its items
        private async Task UpdateOrderTotal(int orderId)
        {
            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

            decimal total = orderItems.Sum(oi => oi.Quantity * oi.UnitPrice);

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.TotalAmount = total;
                await _context.SaveChangesAsync();
            }
        }
    }

    // Helper class for adding multiple order items
    public class MultipleOrderItemsRequest
    {
        public int OrderId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}