using Microsoft.AspNetCore.Authorization;
using GlassyStore.Models;
using GlassyStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace GlassyStore.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ProductsController : Controller
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        // GET: PRODUCTS
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetProductsAsync());
        }

        // GET: PRODUCTS/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? productid)
        {
            if (productid == null)
            {
                return NotFound();
            }

            var product = await _service.GetProductAsync(productid.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: PRODUCTS/Create
       
        public IActionResult Create()
        {
            return View();
        }

        // POST: PRODUCTS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Brand,FrameType,Material,Price,StockQuantity,ImageUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                await _service.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: PRODUCTS/Edit/5
        public async Task<IActionResult> Edit(int? productid)
        {
            if (productid == null)
            {
                return NotFound();
            }

            var product = await _service.GetProductAsync(productid.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: PRODUCTS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? productid, [Bind("ProductId,Name,Brand,FrameType,Material,Price,StockQuantity,ImageUrl")] Product product)
        {
            if (productid != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateProductAsync(product);
                }
                catch
                {
                    if (!await ProductExists(product.ProductId))
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
            return View(product);
        }

        // GET: PRODUCTS/Delete/5
        public async Task<IActionResult> Delete(int? productid)
        {
            if (productid == null)
            {
                return NotFound();
            }

            var product = await _service.GetProductAsync(productid.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: PRODUCTS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? productid)
        {
            if (productid != null)
            {
                await _service.DeleteProductAsync(productid.Value);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(int? productid)
        {
            if (productid == null) return false;
            return await _service.GetProductAsync(productid.Value) != null;
        }
    }
}
