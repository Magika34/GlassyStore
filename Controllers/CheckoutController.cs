using GlassyStore.Data;
using GlassyStore.Models;
using GlassyStore.Services;
using GlassyStore.ViewModels;
using GlassyStore.Services.Email;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GlassyStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly UserManager<IdentityUser> _userManager;

        public CheckoutController(
          ApplicationDbContext context,
          IOrderService orderService,
          IEmailService emailService,
          UserManager<IdentityUser> userManager)
        {
            _context = context;
            _orderService = orderService;
            _emailService = emailService;
            _userManager = userManager;
        }
        public IActionResult Index(int productId = 0)
        {
            if (productId == 0)
            {
                return RedirectToAction("Index", "Products");
            }

            var model = new CheckoutViewModel
            {
                ProductId = productId
            };

            ViewBag.Lenses = new SelectList(
                _context.LensOptions,
                "LensOptionId",
                "Type");

            // provide lens option types for client-side UI toggling
            ViewBag.LensOptionTypes = _context.LensOptions
                .Select(lo => new { lo.LensOptionId, lo.Type })
                .ToList();

            var selected = _context.Products.Find(productId);
            ViewBag.SelectedProduct = selected;

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            // business validation: require prescription unless lens type indicates non-prescription
            var selectedLens = await _context.LensOptions.FindAsync(model.LensOptionId);
            var prescriptionRequired = true;
            if (selectedLens != null && !string.IsNullOrEmpty(selectedLens.Type) && selectedLens.Type.ToLower().Contains("non"))
            {
                prescriptionRequired = false;
            }

            if (prescriptionRequired)
            {
                if (model.PrescriptionSphereLeft == null || model.PrescriptionSphereRight == null
                    || model.PrescriptionCylinderLeft == null || model.PrescriptionCylinderRight == null
                    || model.PupillaryDistance == null)
                {
                    ModelState.AddModelError("", "Prescription values are required for the selected lens type.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Products = new SelectList(
                    _context.Products,
                    "ProductId",
                    "Name");

                ViewBag.Lenses = new SelectList(
                    _context.LensOptions,
                    "LensOptionId",
                    "Type");

                if (model.ProductId > 0)
                {
                    ViewBag.SelectedProduct = _context.Products.Find(model.ProductId);
                }

                return View(model);
            }

            // ensure product exists and is in stock
            var product = await _context.Products.FindAsync(model.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            if (!await _orderService.IsInStockAsync(model.ProductId))
            {
                ModelState.AddModelError("", "Selected frame is out of stock.");

                ViewBag.Products = new SelectList(
                    _context.Products,
                    "ProductId",
                    "Name");

                ViewBag.Lenses = new SelectList(
                    _context.LensOptions,
                    "LensOptionId",
                    "Type");

                if (model.ProductId > 0)
                {
                    ViewBag.SelectedProduct = _context.Products.Find(model.ProductId);
                }

                return View(model);
            }

            // create order item and calculate total (use 0 defaults for non-prescription)
            var orderItem = new OrderItem
            {
                ProductId = model.ProductId,
                LensOptionId = model.LensOptionId,
                PrescriptionSphereLeft = model.PrescriptionSphereLeft ?? 0m,
                PrescriptionSphereRight = model.PrescriptionSphereRight ?? 0m,
                PrescriptionCylinderLeft = model.PrescriptionCylinderLeft ?? 0m,
                PrescriptionCylinderRight = model.PrescriptionCylinderRight ?? 0m,
                PupillaryDistance = model.PupillaryDistance ?? 0
            };

            decimal total = await _orderService.CalculateTotalAsync(orderItem);
            var order = new Order {
                CustomerId = _userManager.GetUserId(User),
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = total
            };

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            orderItem.OrderId = order.OrderId;

            _context.OrderItems.Add(orderItem);

            await _context.SaveChangesAsync();

            // send confirmation email after order is created (use user's email)
            var user = await _userManager.GetUserAsync(User);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                await _emailService.SendEmail(
                  user.Email!,
                  "Order Confirmation",
                  $"""

                  Thank you for shopping with GlassyStore!

                  Order Number: {order.OrderId}

                  Total Amount: {order.TotalAmount:C}

                  Status: {order.Status}

                  Your order has been received successfully.

                  """
                );
            }

            await _orderService.ReduceStockAsync(model.ProductId);

            return View("Success", order);
        }
    }
}