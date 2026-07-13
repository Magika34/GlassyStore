using GlassyStore.Data;
using GlassyStore.Models;
using GlassyStore.Services;
using GlassyStore.ViewModels;
using GlassyStore.Services.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GlassyStore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;

        public CheckoutController(
          ApplicationDbContext context,
          IOrderService orderService,
          IEmailService emailService)
        {
            _context = context;
            _orderService = orderService;
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            ViewBag.Products = new SelectList(
                _context.Products,
                "ProductId",
                "Name");

            ViewBag.Lenses = new SelectList(
                _context.LensOptions,
                "LensOptionId",
                "Type");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
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

                return View(model);
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

                return View(model);
            }
            // create order item and calculate total

            var orderItem = new OrderItem
            {
                ProductId = model.ProductId,
                LensOptionId = model.LensOptionId,
                PrescriptionSphereLeft = model.PrescriptionSphereLeft,
                PrescriptionSphereRight = model.PrescriptionSphereRight,
                PrescriptionCylinderLeft = model.PrescriptionCylinderLeft,
                PrescriptionCylinderRight = model.PrescriptionCylinderRight,
                PupillaryDistance = model.PupillaryDistance
            };

            decimal total =
                await _orderService.CalculateTotalAsync(orderItem);

            var order = new Order
            {
                CustomerId = "Demo Customer",
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = total
            };

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            orderItem.OrderId = order.OrderId;

            _context.OrderItems.Add(orderItem);

            await _context.SaveChangesAsync();

            // send confirmation email after order is created
            await _emailService.SendEmail(
              User.Identity!.Name!,
              "Order Confirmation",
              $"""

              Thank you for shopping with GlassyStore!

              Order Number: {order.OrderId}

              Total Amount: {order.TotalAmount:C}

              Status: {order.Status}

              Your order has been received successfully.

              """
            );

            await _orderService.ReduceStockAsync(model.ProductId);

            return RedirectToAction("Index", "Orders");
        }
    }
}