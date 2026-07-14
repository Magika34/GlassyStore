
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlassyStore.Models;
using GlassyStore.Data;
using GlassyStore.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GlassyStore.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(ApplicationDbContext context, IEmailService emailService, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);

            var orders = await _context.Orders
                .Where(o => o.CustomerId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.LensOption)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // GET: ORDERS
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        // GET: ORDERS/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? orderid)
        {
            if (orderid == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderId == orderid);
            if (order == null)
            {
                return NotFound();
            }

            // allow admin or the order owner to view details
            var userId = _userManager.GetUserId(User);
            if (User.IsInRole("Administrator") || order.CustomerId == userId)
            {
                return View(order);
            }

            return Forbid();
        }
        // GET: ORDERS/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ORDERS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,OrderDate,TotalAmount,Status,RowVersion,OrderItems")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: ORDERS/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? orderid)
        {
            if (orderid == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(orderid);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: ORDERS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? orderid, [Bind("OrderId,CustomerId,OrderDate,TotalAmount,Status,RowVersion,OrderItems")] Order order)
        {
            if (orderid != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            return View(order);
        }

        // GET: ORDERS/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? orderid)
        {
            if (orderid == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderId == orderid);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: ORDERS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int? orderid)
        {
            var order = await _context.Orders.FindAsync(orderid);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int? orderid)
        {
            return _context.Orders.Any(e => e.OrderId == orderid);
        }

    }
}