
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EsnafKar.Models;
using EsnafKar.Data;

public class IsletmelerController : Controller
{
    private readonly AppDbContext _context;

    public IsletmelerController(AppDbContext context)
    {
        _context = context;
    }

    // GET: ISLETMES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Isletmeler.ToListAsync());
    }

    // GET: ISLETMES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var isletme = await _context.Isletmeler
            .FirstOrDefaultAsync(m => m.Id == id);
        if (isletme == null)
        {
            return NotFound();
        }

        return View(isletme);
    }

    // GET: ISLETMES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ISLETMES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Ad,Tur,OlusturmaTarihi")] Isletme isletme)
    {
        if (ModelState.IsValid)
        {
            _context.Add(isletme);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(isletme);
    }

    // GET: ISLETMES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var isletme = await _context.Isletmeler.FindAsync(id);
        if (isletme == null)
        {
            return NotFound();
        }
        return View(isletme);
    }

    // POST: ISLETMES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Ad,Tur,OlusturmaTarihi")] Isletme isletme)
    {
        if (id != isletme.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(isletme);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsletmeExists(isletme.Id))
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
        return View(isletme);
    }

    // GET: ISLETMES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var isletme = await _context.Isletmeler
            .FirstOrDefaultAsync(m => m.Id == id);
        if (isletme == null)
        {
            return NotFound();
        }

        return View(isletme);
    }

    // POST: ISLETMES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var isletme = await _context.Isletmeler.FindAsync(id);
        if (isletme != null)
        {
            _context.Isletmeler.Remove(isletme);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool IsletmeExists(int? id)
    {
        return _context.Isletmeler.Any(e => e.Id == id);
    }
}
