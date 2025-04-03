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
    public class OrderItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderItems
        /// curl -X "GET" https://localhost:5234/api/OrderItems
        /// <summary>
        /// Returns a list of all OrderItems
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{OrderItemDTO},{OrderItemDTO},...]
        /// </returns>
        /// <example>
        /// GET: api/OrderItems -> [{OrderItemDTO},{OrderItemDTO},...]
        /// </example>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderItemDTO>>> GetOrderItems()
        {
            var orderItems =  await _context.OrderItems
                .Include(oi => oi.Order)
                .ToListAsync();
            
            var orderItemDTOs = orderItems.Select(oi => new OrderItemDTO
            {
                ItemId = oi.ItemId,
                OrderId = oi.OrderId,
                ItemName = oi.ItemName,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList();

            return orderItemDTOs;
        }

        // GET: api/OrderItems/5
        /// curl -X "GET" https://localhost:5234/api/OrderItems/5
        /// <summary>
        /// Returns a single OrderItem specified by its {id}
        /// </summary>
        /// <param name="id">The order item id</param>
        /// <returns>
        /// 200 OK
        /// {OrderItemDTO}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/OrderItems/5 -> {OrderItemDTO}
        /// </example>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderItemDTO>> GetOrderItem(int id)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.ItemId == id);

            if (orderItem == null)
            {
                return NotFound();
            }

            var orderItemDTO = new OrderItemDTO{
                ItemId = orderItem.ItemId,
                OrderId = orderItem.OrderId,
                ItemName = orderItem.ItemName,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice
            };

            return orderItemDTO;
        }

        // GET: api/OrderItems/ByOrder/3
        /// curl -X "GET" https://localhost:5234/api/OrderItems/ByOrder/3
        /// <summary>
        /// Returns all OrderItems for a specific Order
        /// </summary>
        /// <param name="orderId">The order id</param>
        /// <returns>
        /// 200 OK
        /// [{OrderItemDTO},{OrderItemDTO},...]
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/OrderItems/ByOrder/3 -> [{OrderItemDTO},{OrderItemDTO},...]
        /// </example>
        [HttpGet("ByOrder/{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<OrderItemDTO>>> GetOrderItemsByOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

             var orderItemDTOs = orderItems.Select(oi => new OrderItemDTO
            {
                ItemId = oi.ItemId,
                OrderId = oi.OrderId,
                ItemName = oi.ItemName,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList();

            return orderItemDTOs;

        }

        // POST: api/OrderItems
        /// curl -X "POST" https://localhost:5234/api/OrderItems -H "Content-Type: application/json" -d '{"OrderId":3,"ItemName":"Pizza","Quantity":2,"UnitPrice":12.99}'
        /// <summary>
        /// Creates a new OrderItem
        /// </summary>
        /// <param name="orderItemDTO">The order item details</param>
        /// <returns>
        /// 201 Created
        /// {OrderItemDTO}
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// POST: api/OrderItems -> {OrderItemDTO}
        /// </example>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderItemDTO>> CreateOrderItem(OrderItemDTO orderItemDTO)
        {
            var order = await _context.Orders.FindAsync(orderItemDTO.OrderId);
            if (order == null)
            {
                return BadRequest("Invalid Order ID");
            }

            var orderItem = new OrderItem{
                ItemId = orderItemDTO.ItemId,
                OrderId = orderItemDTO.OrderId,
                ItemName = orderItemDTO.ItemName,
                Quantity = orderItemDTO.Quantity,
                UnitPrice = orderItemDTO.UnitPrice
            };

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            await UpdateOrderTotal(orderItemDTO.OrderId);

           orderItemDTO.ItemId = orderItem.ItemId;

            return CreatedAtAction(
                nameof(GetOrderItem),
                new { id = orderItem.ItemId },
                orderItem);
        }

        // PUT: api/OrderItems/5
        /// curl -X "PUT" https://localhost:5234/api/OrderItems/5 -H "Content-Type: application/json" -d '{"ItemId":5,"OrderId":3,"ItemName":"Pizza","Quantity":3,"UnitPrice":12.99}'
        /// <summary>
        /// Updates an existing OrderItem
        /// </summary>
        /// <param name="id">The order item id</param>
        /// <param name="orderItemDTO">The updated order item details</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// PUT: api/OrderItems/5 -> 204 No Content
        /// </example>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderItem(int id, OrderItemDTO orderItemDTO)
        {
            if (id != orderItemDTO.ItemId)
            {
                return BadRequest();
            }

            var order = await _context.Orders.FindAsync(orderItemDTO.OrderId);
            if (order == null)
            {
                return BadRequest("Invalid Order ID");
            }

            var orderItem = await _context.OrderItems.FindAsync(id);
             if (orderItem == null)
            {
                return NotFound();
            }

            orderItem.OrderId = orderItemDTO.OrderId;
            orderItem.ItemName = orderItemDTO.ItemName;
            orderItem.Quantity = orderItemDTO.Quantity;
            orderItem.UnitPrice = orderItemDTO.UnitPrice;

            _context.Entry(orderItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                
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
        /// curl -X "DELETE" https://localhost:5234/api/OrderItems/5
        /// <summary>
        /// Deletes an OrderItem
        /// </summary>
        /// <param name="id">The order item id</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/OrderItems/5 -> 204 No Content
        /// </example>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            
            await UpdateOrderTotal(orderId);

            return NoContent();
        }

        // POST: api/OrderItems/AddMultiple
        /// curl -X "POST" https://localhost:5234/api/OrderItems/AddMultiple -H "Content-Type: application/json" -d '{"OrderId":3,"OrderItems":[{"ItemName":"Pizza","Quantity":2,"UnitPrice":12.99},{"ItemName":"Soda","Quantity":1,"UnitPrice":2.50}]}'
        /// <summary>
        /// Creates multiple OrderItems for a single Order
        /// </summary>
        /// <param name="request">The MultipleOrderItemsRequestDTO containing order id and item details</param>
        /// <returns>
        /// 201 Created
        /// [{OrderItemDTO},{OrderItemDTO},...]
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// POST: api/OrderItems/AddMultiple -> [{OrderItemDTO},{OrderItemDTO},...]
        [HttpPost("AddMultiple")]
        public async Task<ActionResult<IEnumerable<OrderItemDTO>>> AddMultipleOrderItems(
            [FromBody] MultipleOrderItemsRequestDTO request)
        {
            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null)
            {
                return BadRequest("Invalid Order ID");
            }

            var orderItems = new List<OrderItem>();

            foreach (var itemDTO in request.OrderItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = request.OrderId,
                    ItemName = itemDTO.ItemName,
                    Quantity = itemDTO.Quantity,
                    UnitPrice = itemDTO.UnitPrice
                };

                orderItems.Add(orderItem);
                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();
            
            await UpdateOrderTotal(request.OrderId);

            var orderItemDTOs = orderItems.Select(oi => new OrderItemDTO
            {
                ItemId = oi.ItemId,
                OrderId = oi.OrderId,
                ItemName = oi.ItemName,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList();

            return CreatedAtAction(
                nameof(GetOrderItemsByOrder),
                new { orderId = request.OrderId },
                orderItemDTOs);
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.ItemId == id);
        }

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

    public class MultipleOrderItemsRequestDTO
    {
        public int OrderId { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    }
}