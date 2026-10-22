using System.Security.Claims;
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

        // Giriş yapan kullanıcının numarası
        private string KullaniciId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Sadece kullanıcının işletmelerine ait giderler
        private IQueryable<Gider> Giderlerim()
        {
            var kullaniciId = KullaniciId;
            return _context.Giderler.Where(g => g.Isletme!.KullaniciId == kullaniciId);
        }

        // Seçilen işletme bu kullanıcıya mı ait?
        private async Task<bool> IsletmeBenimMi(int isletmeId)
        {
            var kullaniciId = KullaniciId;
            return await _context.Isletmeler.AnyAsync(i => i.Id == isletmeId && i.KullaniciId == kullaniciId);
        }

        // Seçilen kategori ortak mı ya da bu kullanıcının mı?
        private async Task<bool> KategoriBenimMi(int kategoriId)
        {
            var kullaniciId = KullaniciId;
            return await _context.GiderKategorileri.AnyAsync(k =>
                k.Id == kategoriId && (k.KullaniciId == null || k.KullaniciId == kullaniciId));
        }
        // Liste sayfası
        public async Task<IActionResult> Index()
        {
            var giderler = await Giderlerim()
                .Include(g => g.Isletme)
                .Include(g => g.GiderKategorisi)
                .OrderByDescending(g => g.Tarih)
                .ToListAsync();
            return View(giderler);
        }

        // Ekleme sayfasını açar
        public IActionResult Create()
        {
            ListeleriHazirla();
            return View(new Gider());
        }

        // Ekleme formu gönderilince çalışır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IsletmeId,GiderKategorisiId,Tutar,Tarih,Aciklama")] Gider gider)
        {
            if (!await IsletmeBenimMi(gider.IsletmeId))
            {
                ModelState.AddModelError("IsletmeId", "İşletme seçilmelidir.");
            }

            if (!await KategoriBenimMi(gider.GiderKategorisiId))
            {
                ModelState.AddModelError("GiderKategorisiId", "Kategori seçilmelidir.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(gider);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ListeleriHazirla(gider.IsletmeId, gider.GiderKategorisiId);
            return View(gider);
        }

        // Düzenleme sayfasını açar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var gider = await Giderlerim().FirstOrDefaultAsync(g => g.Id == id);
            if (gider == null) return NotFound();

            ListeleriHazirla(gider.IsletmeId, gider.GiderKategorisiId);
            return View(gider);
        }

        // Düzenleme formu gönderilince çalışır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IsletmeId,GiderKategorisiId,Tutar,Tarih,Aciklama")] Gider gider)
        {
            if (id != gider.Id) return NotFound();
            if (!await Giderlerim().AnyAsync(g => g.Id == id)) return NotFound();

            if (!await IsletmeBenimMi(gider.IsletmeId))
            {
                ModelState.AddModelError("IsletmeId", "İşletme seçilmelidir.");
            }

            if (!await KategoriBenimMi(gider.GiderKategorisiId))
            {
                ModelState.AddModelError("GiderKategorisiId", "Kategori seçilmelidir.");
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

        // Silme onay sayfasını açar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var gider = await Giderlerim()
                .Include(g => g.Isletme)
                .Include(g => g.GiderKategorisi)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (gider == null) return NotFound();

            return View(gider);
        }

        // Silme onaylanınca çalışır
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gider = await Giderlerim().FirstOrDefaultAsync(g => g.Id == id);
            if (gider != null)
            {
                _context.Giderler.Remove(gider);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Açılır listeleri doldurur: kullanıcının işletmeleri ve kategoriler
        private void ListeleriHazirla(int? seciliIsletme = null, int? seciliKategori = null)
        {
            var kullaniciId = KullaniciId;
            var isletmeler = _context.Isletmeler
                .Where(i => i.KullaniciId == kullaniciId)
                .OrderBy(i => i.Ad);
            ViewData["IsletmeId"] = new SelectList(isletmeler, "Id", "Ad", seciliIsletme);
            var kategoriler = _context.GiderKategorileri
                .Where(k => k.KullaniciId == null || k.KullaniciId == kullaniciId)
                .OrderBy(k => k.Ad);
            ViewData["GiderKategorisiId"] = new SelectList(kategoriler, "Id", "Ad", seciliKategori);
        }
    }
}