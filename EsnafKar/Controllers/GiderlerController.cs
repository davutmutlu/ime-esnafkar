using EsnafKar.Data;
using EsnafKar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EsnafKar.Controllers
{
    public class GiderlerController : Controller
    {
        private readonly AppDbContext _context;

        public GiderlerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Giderler
        public async Task<IActionResult> Index()
        {
            var giderler = await _context.Giderler
                .Include(g => g.Isletme)
                .Include(g => g.GiderKategorisi)
                .OrderByDescending(g => g.Tarih)
                .ToListAsync();

            return View(giderler);
        }

        // GET: Giderler/Create
        public IActionResult Create()
        {
            ListeleriHazirla();
            return View(new Gider());
        }

        // POST: Giderler/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IsletmeId,GiderKategorisiId,Tutar,Tarih,Aciklama")] Gider gider)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gider);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ListeleriHazirla(gider.IsletmeId, gider.GiderKategorisiId);
            return View(gider);
        }

        // GET: Giderler/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gider = await _context.Giderler.FindAsync(id);
            if (gider == null)
            {
                return NotFound();
            }

            ListeleriHazirla(gider.IsletmeId, gider.GiderKategorisiId);
            return View(gider);
        }

        // POST: Giderler/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IsletmeId,GiderKategorisiId,Tutar,Tarih,Aciklama")] Gider gider)
        {
            if (id != gider.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(gider);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ListeleriHazirla(gider.IsletmeId, gider.GiderKategorisiId);
            return View(gider);
        }

        // GET: Giderler/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gider = await _context.Giderler
                .Include(g => g.Isletme)
                .Include(g => g.GiderKategorisi)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (gider == null)
            {
                return NotFound();
            }

            return View(gider);
        }

        // POST: Giderler/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gider = await _context.Giderler.FindAsync(id);
            if (gider != null)
            {
                _context.Giderler.Remove(gider);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void ListeleriHazirla(int? seciliIsletme = null, int? seciliKategori = null)
        {
            ViewData["IsletmeId"] = new SelectList(
                _context.Isletmeler.OrderBy(i => i.Ad), "Id", "Ad", seciliIsletme);

            ViewData["GiderKategorisiId"] = new SelectList(
                _context.GiderKategorileri.OrderBy(k => k.Ad), "Id", "Ad", seciliKategori);
        }
    }
}