using System.Security.Claims;
using EsnafKar.Data;
using EsnafKar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EsnafKar.Controllers
{
    public class KarRaporuController : Controller
    {
        private readonly AppDbContext _context;

        public KarRaporuController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? isletmeId, int? yil, int? ay)
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Seçilen işletme başkasınınsa yokmuş gibi davran
            if (isletmeId != null &&
                !await _context.Isletmeler.AnyAsync(i => i.Id == isletmeId && i.KullaniciId == kullaniciId))
            {
                isletmeId = null;
            }

            var rapor = new KarRaporu
            {
                IsletmeId = isletmeId,
                Yil = yil ?? DateTime.Today.Year,
                Ay = ay ?? DateTime.Today.Month
            };

            if (rapor.Ay < 1 || rapor.Ay > 12) rapor.Ay = DateTime.Today.Month;
            if (rapor.Yil < 2000 || rapor.Yil > 2100) rapor.Yil = DateTime.Today.Year;

            var isletmeler = _context.Isletmeler
                .Where(i => i.KullaniciId == kullaniciId)
                .OrderBy(i => i.Ad);
            ViewData["IsletmeId"] = new SelectList(isletmeler, "Id", "Ad", isletmeId);

            if (isletmeId == null)
            {
                return View(rapor);
            }

            var baslangic = new DateTime(rapor.Yil, rapor.Ay, 1);
            var bitis = baslangic.AddMonths(1);
            var oncekiBaslangic = baslangic.AddMonths(-1);

            // Bu ay
            rapor.ToplamGelir = await _context.Gelirler
                .Where(g => g.IsletmeId == isletmeId && g.Tarih >= baslangic && g.Tarih < bitis)
                .SumAsync(g => g.Tutar);

            var giderler = _context.Giderler
                .Where(g => g.IsletmeId == isletmeId && g.Tarih >= baslangic && g.Tarih < bitis);

            rapor.ToplamGider = await giderler.SumAsync(g => g.Tutar);

            // Geçen ay
            rapor.OncekiGelir = await _context.Gelirler
                .Where(g => g.IsletmeId == isletmeId && g.Tarih >= oncekiBaslangic && g.Tarih < baslangic)
                .SumAsync(g => g.Tutar);

            rapor.OncekiGider = await _context.Giderler
                .Where(g => g.IsletmeId == isletmeId && g.Tarih >= oncekiBaslangic && g.Tarih < baslangic)
                .SumAsync(g => g.Tutar);

            // Gider dağılımı
            rapor.GiderDagilimi = await giderler
                .GroupBy(g => g.GiderKategorisi!.Ad)
                .Select(grup => new KategoriToplami
                {
                    Kategori = grup.Key,
                    Tutar = grup.Sum(g => g.Tutar)
                })
                .OrderByDescending(k => k.Tutar)
                .ToListAsync();

            return View(rapor);
        }
    }
}