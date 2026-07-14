using GlassyStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlassyStore.Extensions;

namespace GlassyStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cart = HttpContext.Session.GetObject<List<int>>("Cart")
                       ?? new List<int>();

            var products = await _context.Products
                .Where(p => cart.Contains(p.ProductId))
                .ToListAsync();

            return View(products);
        }

        public IActionResult Add(int id)
        {
            var cart = HttpContext.Session.GetObject<List<int>>("Cart")
                       ?? new List<int>();

            if (!cart.Contains(id))
                cart.Add(id);

            HttpContext.Session.SetObject("Cart", cart);

            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<int>>("Cart")
                       ?? new List<int>();

            cart.Remove(id);

            HttpContext.Session.SetObject("Cart", cart);

            return RedirectToAction("Index");
        }
    }
}
