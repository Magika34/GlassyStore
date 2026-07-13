
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlassyStore.Models;
using GlassyStore.Data;
using GlassyStore.Services.Email;
using Microsoft.AspNetCore.Identity;

namespace GlassyStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public OrdersController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: ORDERS
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        // GET: ORDERS/Details/5
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

            return View(order);
        }
        // GET: ORDERS/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ORDERS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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