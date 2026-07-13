
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlassyStore.Models;
using GlassyStore.Data;

public class OrderItemsController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrderItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: ORDERITEMS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.OrderItems.ToListAsync());
    }

    // GET: ORDERITEMS/Details/5
    public async Task<IActionResult> Details(int? orderitemid)
    {
        if (orderitemid == null)
        {
            return NotFound();
        }

        var orderitem = await _context.OrderItems
            .FirstOrDefaultAsync(m => m.OrderItemId == orderitemid);
        if (orderitem == null)
        {
            return NotFound();
        }

        return View(orderitem);
    }

    // GET: ORDERITEMS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ORDERITEMS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("OrderItemId,OrderId,Order,ProductId,Product,LensOptionId,LensOption,PrescriptionSphereLeft,PrescriptionSphereRight,PrescriptionCylinderLeft,PrescriptionCylinderRight,PupillaryDistance")] OrderItem orderitem)
    {
        if (ModelState.IsValid)
        {
            _context.Add(orderitem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(orderitem);
    }

    // GET: ORDERITEMS/Edit/5
    public async Task<IActionResult> Edit(int? orderitemid)
    {
        if (orderitemid == null)
        {
            return NotFound();
        }

        var orderitem = await _context.OrderItems.FindAsync(orderitemid);
        if (orderitem == null)
        {
            return NotFound();
        }
        return View(orderitem);
    }

    // POST: ORDERITEMS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? orderitemid, [Bind("OrderItemId,OrderId,Order,ProductId,Product,LensOptionId,LensOption,PrescriptionSphereLeft,PrescriptionSphereRight,PrescriptionCylinderLeft,PrescriptionCylinderRight,PupillaryDistance")] OrderItem orderitem)
    {
        if (orderitemid != orderitem.OrderItemId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(orderitem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderItemExists(orderitem.OrderItemId))
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
        return View(orderitem);
    }

    // GET: ORDERITEMS/Delete/5
    public async Task<IActionResult> Delete(int? orderitemid)
    {
        if (orderitemid == null)
        {
            return NotFound();
        }

        var orderitem = await _context.OrderItems
            .FirstOrDefaultAsync(m => m.OrderItemId == orderitemid);
        if (orderitem == null)
        {
            return NotFound();
        }

        return View(orderitem);
    }

    // POST: ORDERITEMS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? orderitemid)
    {
        var orderitem = await _context.OrderItems.FindAsync(orderitemid);
        if (orderitem != null)
        {
            _context.OrderItems.Remove(orderitem);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool OrderItemExists(int? orderitemid)
    {
        return _context.OrderItems.Any(e => e.OrderItemId == orderitemid);
    }
}
