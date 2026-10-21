using System.Security.Claims;
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

        // Giriş yapan kullanıcının numarası
        private string KullaniciId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Sadece kullanıcının işletmelerine ait gelirler
        private IQueryable<Gelir> Gelirlerim()
        {
            var kullaniciId = KullaniciId;
            return _context.Gelirler.Where(g => g.Isletme!.KullaniciId == kullaniciId);
        }

        // Seçilen işletme bu kullanıcıya mı ait?
        private async Task<bool> IsletmeBenimMi(int isletmeId)
        {
            var kullaniciId = KullaniciId;
            return await _context.Isletmeler.AnyAsync(i => i.Id == isletmeId && i.KullaniciId == kullaniciId);
        }

        // Liste sayfası
        public async Task<IActionResult> Index()
        {
            var gelirler = await Gelirlerim()
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
            if (!await IsletmeBenimMi(gelir.IsletmeId))
            {
                ModelState.AddModelError("IsletmeId", "İşletme seçilmelidir.");
            }

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

            var gelir = await Gelirlerim().FirstOrDefaultAsync(g => g.Id == id);
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
            if (!await Gelirlerim().AnyAsync(g => g.Id == id)) return NotFound();

            if (!await IsletmeBenimMi(gelir.IsletmeId))
            {
                ModelState.AddModelError("IsletmeId", "İşletme seçilmelidir.");
            }

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

            var gelir = await Gelirlerim()
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
            var gelir = await Gelirlerim().FirstOrDefaultAsync(g => g.Id == id);
            if (gelir != null)
            {
                _context.Gelirler.Remove(gelir);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Açılır listeye sadece kullanıcının işletmelerini doldurur
        private void IsletmeListesiniHazirla(int? seciliIsletme = null)
        {
            var kullaniciId = KullaniciId;
            var isletmeler = _context.Isletmeler
                .Where(i => i.KullaniciId == kullaniciId)
                .OrderBy(i => i.Ad);
            ViewData["IsletmeId"] = new SelectList(isletmeler, "Id", "Ad", seciliIsletme);
        }
    }
}