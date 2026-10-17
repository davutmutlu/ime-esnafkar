using EsnafKar.Data;
using EsnafKar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EsnafKar.Controllers
{
    public class GelirlerController : Controller
    {
        private readonly AppDbContext _context;

        public GelirlerController(AppDbContext context)
        {
            _context = context;
        }

        // Liste sayfası
        public async Task<IActionResult> Index()
        {
            var gelirler = await _context.Gelirler
                .Include(g => g.Isletme)
                .OrderByDescending(g => g.Tarih)
                .ToListAsync();
            return View(gelirler);
        }

        // Ekleme sayfasını açar
        public IActionResult Create()
        {
            IsletmeListesiniHazirla();
            return View(new Gelir());
        }

        // Ekleme formu gönderilince çalışır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IsletmeId,Tarih,Tutar,Aciklama")] Gelir gelir)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gelir);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            IsletmeListesiniHazirla(gelir.IsletmeId);
            return View(gelir);
        }

        // Düzenleme sayfasını açar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var gelir = await _context.Gelirler.FindAsync(id);
            if (gelir == null) return NotFound();

            IsletmeListesiniHazirla(gelir.IsletmeId);
            return View(gelir);
        }

        // Düzenleme formu gönderilince çalışır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IsletmeId,Tarih,Tutar,Aciklama")] Gelir gelir)
        {
            if (id != gelir.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(gelir);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            IsletmeListesiniHazirla(gelir.IsletmeId);
            return View(gelir);
        }

        // Silme onay sayfasını açar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var gelir = await _context.Gelirler
                .Include(g => g.Isletme)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (gelir == null) return NotFound();

            return View(gelir);
        }

        // Silme onaylanınca çalışır
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gelir = await _context.Gelirler.FindAsync(id);
            if (gelir != null)
            {
                _context.Gelirler.Remove(gelir);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Açılır listeye işletmeleri doldurur
        private void IsletmeListesiniHazirla(int? seciliIsletme = null)
        {
            ViewData["IsletmeId"] = new SelectList(_context.Isletmeler.OrderBy(i => i.Ad), "Id", "Ad", seciliIsletme);
        }
    }
}