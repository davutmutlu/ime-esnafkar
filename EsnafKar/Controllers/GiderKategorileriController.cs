using System.Security.Claims;
using EsnafKar.Data;
using EsnafKar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EsnafKar.Controllers
{
    public class GiderKategorileriController : Controller
    {
        private readonly AppDbContext _context;

        public GiderKategorileriController(AppDbContext context)
        {
            _context = context;
        }

        // Giriş yapan kullanıcının numarası
        private string KullaniciId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Kullanıcının kendi eklediği kategoriler
        private IQueryable<GiderKategorisi> Kategorilerim()
        {
            var kullaniciId = KullaniciId;
            return _context.GiderKategorileri.Where(k => k.KullaniciId == kullaniciId);
        }

        // Liste: ortak kategoriler ve kullanıcının kendi kategorileri
        public async Task<IActionResult> Index()
        {
            var kullaniciId = KullaniciId;
            var kategoriler = await _context.GiderKategorileri
                .Where(k => k.KullaniciId == null || k.KullaniciId == kullaniciId)
                .OrderBy(k => k.Ad)
                .ToListAsync();
            return View(kategoriler);
        }

        // Ekleme formunu açar
        public IActionResult Create()
        {
            return View();
        }

        // Ekleme formu gönderilince çalışır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ad")] GiderKategorisi giderKategorisi)
        {
            if (!ModelState.IsValid) return View(giderKategorisi);

            giderKategorisi.KullaniciId = KullaniciId;
            _context.Add(giderKategorisi);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Düzenleme formunu açar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var kategori = await Kategorilerim().FirstOrDefaultAsync(k => k.Id == id);
            if (kategori == null) return NotFound();

            return View(kategori);
        }

        // Düzenleme formu gönderilince çalışır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad")] GiderKategorisi giderKategorisi)
        {
            if (id != giderKategorisi.Id) return NotFound();
            if (!ModelState.IsValid) return View(giderKategorisi);

            var kayit = await Kategorilerim().FirstOrDefaultAsync(k => k.Id == id);
            if (kayit == null) return NotFound();

            kayit.Ad = giderKategorisi.Ad;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Silme onay sayfasını açar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var kategori = await Kategorilerim().FirstOrDefaultAsync(k => k.Id == id);
            if (kategori == null) return NotFound();

            return View(kategori);
        }

        // Silme onaylanınca çalışır
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool kullaniliyor = await _context.Giderler.AnyAsync(g => g.GiderKategorisiId == id);
            if (kullaniliyor)
            {
                TempData["Hata"] = "Bu kategoriye ait giderler olduğu için silinemez. Önce bu kategorideki giderleri silmelisiniz.";
                return RedirectToAction(nameof(Index));
            }

            var kategori = await Kategorilerim().FirstOrDefaultAsync(k => k.Id == id);
            if (kategori != null)
            {
                _context.GiderKategorileri.Remove(kategori);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}