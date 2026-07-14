using GlassyStore.Data;
using GlassyStore.Models;
using GlassyStore.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassyStore.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, IEmailService emailService, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> EditStatus(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(Order order)
        {
            if (!ModelState.IsValid) return View(order);

            var dbOrder = await _context.Orders.FindAsync(order.OrderId);
            if (dbOrder == null) return NotFound();

            dbOrder.Status = order.Status;
            _context.Update(dbOrder);
            await _context.SaveChangesAsync();

            if (dbOrder.Status == "Shipped")
            {
                // try to lookup customer email by user id stored in CustomerId
                var user = await _userManager.FindByIdAsync(dbOrder.CustomerId);
                var email = user?.Email ?? dbOrder.CustomerId; // fallback

                await _emailService.SendEmail(
                    email,
                    "Your Glasses Have Been Shipped",
                    $"""

                    Great news!

                    Your glasses have been shipped.

                    Order Number: {dbOrder.OrderId}

                    Status: Shipped

                    Thank you for shopping with GlassyStore.

                    """
                );
            }

            return RedirectToAction(nameof(Dashboard));
        }
    }
}
