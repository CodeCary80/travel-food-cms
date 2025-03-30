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
    public class OrdersPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrdersPage
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Restaurant)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                RestaurantId = o.RestaurantId,
                RestaurantName = o.Restaurant?.Name,
                UserId = o.UserId,
                UserName = o.User?.Username,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                TotalItemCount = o.OrderItems?.Sum(oi => oi.Quantity) ?? 0
            }).ToList();

            return View(orderViewModels);
        }

        // GET: OrdersPage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Restaurant)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderViewModel = new OrderViewModel
            {
                OrderId = order.OrderId,
                RestaurantId = order.RestaurantId,
                RestaurantName = order.Restaurant?.Name,
                UserId = order.UserId,
                UserName = order.User?.Username,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                SpecialRequests = order.SpecialRequests,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemViewModel
                {
                    ItemId = oi.ItemId,
                    ItemName = oi.ItemName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.Quantity * oi.UnitPrice
                }).ToList()
            };

            return View(orderViewModel);
        }

        // GET: OrdersPage/Create
        public IActionResult Create()
        {
            // Populate dropdowns for restaurants and users
            ViewBag.Restaurants = _context.Restaurants
                .Select(r => new { r.RestaurantId, r.Name })
                .ToList();

            ViewBag.Users = _context.Users
                .Select(u => new { u.UserId, u.Username })
                .ToList();

            return View(new OrderViewModel());
        }

        // POST: OrdersPage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel orderViewModel)
        {
            if (ModelState.IsValid)
            {
                var order = new Order
                {
                    RestaurantId = orderViewModel.RestaurantId,
                    UserId = orderViewModel.UserId,
                    OrderDate = DateTime.Now,
                    TotalAmount = orderViewModel.TotalAmount,
                    Status = orderViewModel.Status ?? "Pending",
                    SpecialRequests = orderViewModel.SpecialRequests
                };

                _context.Add(order);
                await _context.SaveChangesAsync();

                // Optionally add order items if provided
                if (orderViewModel.OrderItems != null)
                {
                    foreach (var itemViewModel in orderViewModel.OrderItems)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = order.OrderId,
                            ItemName = itemViewModel.ItemName,
                            Quantity = itemViewModel.Quantity,
                            UnitPrice = itemViewModel.UnitPrice
                        };
                        _context.OrderItems.Add(orderItem);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdowns if model is invalid
            ViewBag.Restaurants = _context.Restaurants
                .Select(r => new { r.RestaurantId, r.Name })
                .ToList();

            ViewBag.Users = _context.Users
                .Select(u => new { u.UserId, u.Username })
                .ToList();

            return View(orderViewModel);
        }

        // GET: OrdersPage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            // Populate dropdowns
            ViewBag.Restaurants = _context.Restaurants
                .Select(r => new { r.RestaurantId, r.Name })
                .ToList();

            ViewBag.Users = _context.Users
                .Select(u => new { u.UserId, u.Username })
                .ToList();

            var orderViewModel = new OrderViewModel
            {
                OrderId = order.OrderId,
                RestaurantId = order.RestaurantId,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                SpecialRequests = order.SpecialRequests,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemViewModel
                {
                    ItemId = oi.ItemId,
                    OrderId = oi.OrderId,
                    ItemName = oi.ItemName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };

            return View(orderViewModel);
        }

        // POST: OrdersPage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderViewModel orderViewModel)
        {
            if (id != orderViewModel.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var order = await _context.Orders
                        .Include(o => o.OrderItems)
                        .FirstOrDefaultAsync(o => o.OrderId == id);

                    if (order == null)
                    {
                        return NotFound();
                    }

                    // Update order details
                    order.RestaurantId = orderViewModel.RestaurantId;
                    order.UserId = orderViewModel.UserId;
                    order.TotalAmount = orderViewModel.TotalAmount;
                    order.Status = orderViewModel.Status;
                    order.SpecialRequests = orderViewModel.SpecialRequests;

                    // Handle OrderItems
                    // Remove existing order items
                    _context.OrderItems.RemoveRange(order.OrderItems);

                    // Add new order items
                    if (orderViewModel.OrderItems != null)
                    {
                        foreach (var itemViewModel in orderViewModel.OrderItems)
                        {
                            var orderItem = new OrderItem
                            {
                                OrderId = order.OrderId,
                                ItemName = itemViewModel.ItemName,
                                Quantity = itemViewModel.Quantity,
                                UnitPrice = itemViewModel.UnitPrice
                            };
                            _context.OrderItems.Add(orderItem);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(orderViewModel.OrderId))
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

            // Repopulate dropdowns if model is invalid
            ViewBag.Restaurants = _context.Restaurants
                .Select(r => new { r.RestaurantId, r.Name })
                .ToList();

            ViewBag.Users = _context.Users
                .Select(u => new { u.UserId, u.Username })
                .ToList();

            return View(orderViewModel);
        }

        // GET: OrdersPage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Restaurant)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderViewModel = new OrderViewModel
            {
                OrderId = order.OrderId,
                RestaurantId = order.RestaurantId,
                RestaurantName = order.Restaurant?.Name,
                UserId = order.UserId,
                UserName = order.User?.Username,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status
            };

            return View(orderViewModel);
        }

        // POST: OrdersPage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}