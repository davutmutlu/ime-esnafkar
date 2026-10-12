
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EsnafKar.Models;
using EsnafKar.Data;

public class GiderKategorileriController : Controller
{
    private readonly AppDbContext _context;

    public GiderKategorileriController(AppDbContext context)
    {
        _context = context;
    }

    // GET: GIDERKATEGORISIS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.GiderKategorileri.ToListAsync());
    }

    // GET: GIDERKATEGORISIS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var giderkategorisi = await _context.GiderKategorileri
            .FirstOrDefaultAsync(m => m.Id == id);
        if (giderkategorisi == null)
        {
            return NotFound();
        }

        return View(giderkategorisi);
    }

    // GET: GIDERKATEGORISIS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: GIDERKATEGORISIS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Ad")] GiderKategorisi giderkategorisi)
    {
        if (ModelState.IsValid)
        {
            _context.Add(giderkategorisi);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(giderkategorisi);
    }

    // GET: GIDERKATEGORISIS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var giderkategorisi = await _context.GiderKategorileri.FindAsync(id);
        if (giderkategorisi == null)
        {
            return NotFound();
        }
        return View(giderkategorisi);
    }

    // POST: GIDERKATEGORISIS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Ad")] GiderKategorisi giderkategorisi)
    {
        if (id != giderkategorisi.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(giderkategorisi);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GiderKategorisiExists(giderkategorisi.Id))
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
        return View(giderkategorisi);
    }

    // GET: GIDERKATEGORISIS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var giderkategorisi = await _context.GiderKategorileri
            .FirstOrDefaultAsync(m => m.Id == id);
        if (giderkategorisi == null)
        {
            return NotFound();
        }

        return View(giderkategorisi);
    }

    // POST: GIDERKATEGORISIS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        bool kullaniliyor = await _context.Giderler.AnyAsync(g => g.GiderKategorisiId == id);
        if (kullaniliyor)
        {
            TempData["Hata"] = "Bu kategoriye ait giderler olduğu için silinemez. Önce bu kategorideki giderleri silmelisiniz.";
            return RedirectToAction(nameof(Index));
        }
        var giderkategorisi = await _context.GiderKategorileri.FindAsync(id);
        if (giderkategorisi != null)
        {
            _context.GiderKategorileri.Remove(giderkategorisi);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool GiderKategorisiExists(int? id)
    {
        return _context.GiderKategorileri.Any(e => e.Id == id);
    }
}
