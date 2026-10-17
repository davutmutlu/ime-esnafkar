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
            var rapor = new KarRaporu
            {
                IsletmeId = isletmeId,
                Yil = yil ?? DateTime.Today.Year,
                Ay = ay ?? DateTime.Today.Month
            };

            if (rapor.Ay < 1 || rapor.Ay > 12) rapor.Ay = DateTime.Today.Month;
            if (rapor.Yil < 2000 || rapor.Yil > 2100) rapor.Yil = DateTime.Today.Year;

            ViewData["IsletmeId"] = new SelectList(_context.Isletmeler.OrderBy(i => i.Ad), "Id", "Ad", isletmeId);

            if (isletmeId == null)
            {
                return View(rapor);
            }

            var baslangic = new DateTime(rapor.Yil, rapor.Ay, 1);
            var bitis = baslangic.AddMonths(1);

            rapor.ToplamGelir = await _context.Gelirler
                .Where(g => g.IsletmeId == isletmeId && g.Tarih >= baslangic && g.Tarih < bitis)
                .SumAsync(g => g.Tutar);

            var giderler = _context.Giderler
                .Where(g => g.IsletmeId == isletmeId && g.Tarih >= baslangic && g.Tarih < bitis);

            rapor.ToplamGider = await giderler.SumAsync(g => g.Tutar);

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