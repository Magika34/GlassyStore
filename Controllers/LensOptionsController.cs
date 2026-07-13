
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlassyStore.Models;
using GlassyStore.Data;

public class LensOptionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public LensOptionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: LENSOPTIONS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.LensOptions.ToListAsync());
    }

    // GET: LENSOPTIONS/Details/5
    public async Task<IActionResult> Details(int? lensoptionid)
    {
        if (lensoptionid == null)
        {
            return NotFound();
        }

        var lensoption = await _context.LensOptions
            .FirstOrDefaultAsync(m => m.LensOptionId == lensoptionid);
        if (lensoption == null)
        {
            return NotFound();
        }

        return View(lensoption);
    }

    // GET: LENSOPTIONS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: LENSOPTIONS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("LensOptionId,Type,Material,Coating,PriceModifier,OrderItems")] LensOption lensoption)
    {
        if (ModelState.IsValid)
        {
            _context.Add(lensoption);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(lensoption);
    }

    // GET: LENSOPTIONS/Edit/5
    public async Task<IActionResult> Edit(int? lensoptionid)
    {
        if (lensoptionid == null)
        {
            return NotFound();
        }

        var lensoption = await _context.LensOptions.FindAsync(lensoptionid);
        if (lensoption == null)
        {
            return NotFound();
        }
        return View(lensoption);
    }

    // POST: LENSOPTIONS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? lensoptionid, [Bind("LensOptionId,Type,Material,Coating,PriceModifier,OrderItems")] LensOption lensoption)
    {
        if (lensoptionid != lensoption.LensOptionId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(lensoption);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LensOptionExists(lensoption.LensOptionId))
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
        return View(lensoption);
    }

    // GET: LENSOPTIONS/Delete/5
    public async Task<IActionResult> Delete(int? lensoptionid)
    {
        if (lensoptionid == null)
        {
            return NotFound();
        }

        var lensoption = await _context.LensOptions
            .FirstOrDefaultAsync(m => m.LensOptionId == lensoptionid);
        if (lensoption == null)
        {
            return NotFound();
        }

        return View(lensoption);
    }

    // POST: LENSOPTIONS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? lensoptionid)
    {
        var lensoption = await _context.LensOptions.FindAsync(lensoptionid);
        if (lensoption != null)
        {
            _context.LensOptions.Remove(lensoption);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool LensOptionExists(int? lensoptionid)
    {
        return _context.LensOptions.Any(e => e.LensOptionId == lensoptionid);
    }
}
