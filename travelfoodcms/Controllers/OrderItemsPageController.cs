using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TravelFoodCms.Data;
using TravelFoodCms.Models;
using TravelFoodCms.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TravelFoodCms.Controllers
{
    [Authorize]
    public class OrderItemsPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderItemsPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderItemsPage
        public async Task<IActionResult> Index()
        {
            var orderItems = await _context.OrderItems
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Restaurant)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.User)
                .ToListAsync();

            var orderItemViewModels = orderItems.Select(oi => new OrderItemViewModel
            {
                ItemId = oi.ItemId,
                OrderId = oi.OrderId,
                ItemName = oi.ItemName,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                OrderDate = oi.Order?.OrderDate,
                RestaurantName = oi.Order?.Restaurant?.Name,
                UserName = oi.Order?.User?.Username
            }).ToList();

            return View(orderItemViewModels);
        }

        // GET: OrderItemsPage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Restaurant)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(m => m.ItemId == id);

            if (orderItem == null)
            {
                return NotFound();
            }

            var orderItemViewModel = new OrderItemViewModel
            {
                ItemId = orderItem.ItemId,
                OrderId = orderItem.OrderId,
                ItemName = orderItem.ItemName,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                OrderDate = orderItem.Order?.OrderDate,
                RestaurantName = orderItem.Order?.Restaurant?.Name,
                UserName = orderItem.Order?.User?.Username
            };

            return View(orderItemViewModel);
        }

        // GET: OrderItemsPage/Create
        public IActionResult Create()
        {
            // Populate order dropdown
            ViewBag.Orders = _context.Orders
                .Select(o => new SelectListItem 
                { 
                    Value = o.OrderId.ToString(), 
                    Text = $"Order {o.OrderId} - {o.Restaurant.Name} - {o.OrderDate:d}" 
                })
                .ToList();

            return View(new OrderItemViewModel());
        }

        // POST: OrderItemsPage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(OrderItemViewModel orderItemViewModel)
        {
            // Remove validations for display properties
            ModelState.Remove("RestaurantName");
            ModelState.Remove("UserName");
            ModelState.Remove("OrderDate");

            if (ModelState.IsValid)
            {
                // Verify the order exists
                var order = await _context.Orders.FindAsync(orderItemViewModel.OrderId);
                if (order == null)
                {
                    ModelState.AddModelError("OrderId", "Invalid Order");
                    
                    // Repopulate order dropdown
                    ViewBag.Orders = _context.Orders
                        .Include(o => o.Restaurant)
                        .Select(o => new SelectListItem 
                        { 
                            Value = o.OrderId.ToString(), 
                            Text = $"Order {o.OrderId} - {o.Restaurant.Name} - {o.OrderDate:d}" 
                        })
                        .ToList();

                    return View(orderItemViewModel);
                }

                var orderItem = new OrderItem
                {
                    OrderId = orderItemViewModel.OrderId,
                    ItemName = orderItemViewModel.ItemName,
                    Quantity = orderItemViewModel.Quantity,
                    UnitPrice = orderItemViewModel.UnitPrice
                };

                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();

                // Update order total
                await UpdateOrderTotal(orderItem.OrderId);

                return RedirectToAction(nameof(Index));
            }

            // Repopulate order dropdown if model is invalid
            ViewBag.Orders = _context.Orders
                .Include(o => o.Restaurant)
                .Select(o => new SelectListItem 
                { 
                    Value = o.OrderId.ToString(), 
                    Text = $"Order {o.OrderId} - {o.Restaurant.Name} - {o.OrderDate:d}" 
                })
                .ToList();

            return View(orderItemViewModel);
        }

        // GET: OrderItemsPage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            // Populate order dropdown
            ViewBag.Orders = _context.Orders
                    .Select(o => new SelectListItem 
                    { 
                        Value = o.OrderId.ToString(), 
                        Text = $"Order {o.OrderId} - {o.Restaurant.Name} - {o.OrderDate:d}" 
                    })
                    .ToList();

            var orderItemViewModel = new OrderItemViewModel
            {
                ItemId = orderItem.ItemId,
                OrderId = orderItem.OrderId,
                ItemName = orderItem.ItemName,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice
            };

            return View(orderItemViewModel);
        }

        // POST: OrderItemsPage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, OrderItemViewModel orderItemViewModel)
        {
            if (id != orderItemViewModel.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var orderItem = await _context.OrderItems.FindAsync(id);
                    if (orderItem == null)
                    {
                        return NotFound();
                    }

                    // Store the original order ID to update totals
                    var originalOrderId = orderItem.OrderId;

                    // Update order item
                    orderItem.OrderId = orderItemViewModel.OrderId;
                    orderItem.ItemName = orderItemViewModel.ItemName;
                    orderItem.Quantity = orderItemViewModel.Quantity;
                    orderItem.UnitPrice = orderItemViewModel.UnitPrice;

                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();

                    // Update totals for both original and new orders
                    await UpdateOrderTotal(originalOrderId);
                    if (originalOrderId != orderItemViewModel.OrderId)
                    {
                        await UpdateOrderTotal(orderItemViewModel.OrderId);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the order item.");
                    
                    // Repopulate orders dropdown
                    ViewBag.Orders = _context.Orders
                        .Select(o => new SelectListItem 
                        { 
                            Value = o.OrderId.ToString(), 
                            Text = $"Order {o.OrderId} - {o.Restaurant.Name} - {o.OrderDate:d}" 
                        })
                        .ToList();

                    return View(orderItemViewModel);
                }
            }

            // Repopulate orders dropdown if model is invalid
            ViewBag.Orders = _context.Orders
                .Select(o => new SelectListItem 
                { 
                    Value = o.OrderId.ToString(), 
                    Text = $"Order {o.OrderId} - {o.Restaurant.Name} - {o.OrderDate:d}" 
                })
                .ToList();

            return View(orderItemViewModel);
        }
        // GET: OrderItemsPage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Restaurant)
                .FirstOrDefaultAsync(m => m.ItemId == id);

            if (orderItem == null)
            {
                return NotFound();
            }

            var orderItemViewModel = new OrderItemViewModel
            {
                ItemId = orderItem.ItemId,
                OrderId = orderItem.OrderId,
                ItemName = orderItem.ItemName,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                RestaurantName = orderItem.Order?.Restaurant?.Name
            };

            return View(orderItemViewModel);
        }

        // POST: OrderItemsPage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            
            if (orderItem == null)
            {
                return NotFound();
            }

            var orderId = orderItem.OrderId;

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            // Update order total after deletion
            await UpdateOrderTotal(orderId);

            return RedirectToAction(nameof(Index));
        }

        // Helper method to update order total
        private async Task UpdateOrderTotal(int orderId)
        {
            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.TotalAmount = orderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
                await _context.SaveChangesAsync();
            }
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.ItemId == id);
        }
    }
}