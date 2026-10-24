using System.Security.Claims;
using ClosedXML.Excel;
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

        // Liste sayfası (filtreli)
        public async Task<IActionResult> Index(int? isletmeId, int? kategoriId, int? yil, int? ay)
        {
            var sorgu = Giderlerim();

            if (isletmeId != null)
            {
                sorgu = sorgu.Where(g => g.IsletmeId == isletmeId);
            }

            if (kategoriId != null)
            {
                sorgu = sorgu.Where(g => g.GiderKategorisiId == kategoriId);
            }

            if (yil != null)
            {
                sorgu = sorgu.Where(g => g.Tarih.Year == yil);
            }

            if (ay != null)
            {
                sorgu = sorgu.Where(g => g.Tarih.Month == ay);
            }

            var giderler = await sorgu
                .Include(g => g.Isletme)
                .Include(g => g.GiderKategorisi)
                .OrderByDescending(g => g.Tarih)
                .ToListAsync();

            ListeleriHazirla(isletmeId, kategoriId);
            ViewData["SeciliYil"] = yil;
            ViewData["SeciliAy"] = ay;
            return View(giderler);
        }

        // Geçen ayın giderlerini bu aya kopyalar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GecenAyiKopyala(int isletmeId)
        {
            if (!await IsletmeBenimMi(isletmeId))
            {
                TempData["Hata"] = "Lütfen bir işletme seçin.";
                return RedirectToAction(nameof(Index));
            }

            var buAyBasi = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var gelecekAyBasi = buAyBasi.AddMonths(1);
            var gecenAyBasi = buAyBasi.AddMonths(-1);
            var gunSayisi = DateTime.DaysInMonth(buAyBasi.Year, buAyBasi.Month);

            var gecenAyGiderleri = await _context.Giderler
                .Where(g => g.IsletmeId == isletmeId && g.Tarih >= gecenAyBasi && g.Tarih < buAyBasi)
                .ToListAsync();

            var buAyKategorileri = await _context.Giderler
                .Where(g => g.IsletmeId == isletmeId && g.Tarih >= buAyBasi && g.Tarih < gelecekAyBasi)
                .Select(g => g.GiderKategorisiId)
                .ToListAsync();

            int sayac = 0;
            foreach (var eski in gecenAyGiderleri)
            {
                if (buAyKategorileri.Contains(eski.GiderKategorisiId)) continue;

                _context.Giderler.Add(new Gider
                {
                    IsletmeId = eski.IsletmeId,
                    GiderKategorisiId = eski.GiderKategorisiId,
                    Tutar = eski.Tutar,
                    Tarih = buAyBasi.AddDays(Math.Min(eski.Tarih.Day, gunSayisi) - 1),
                    Aciklama = eski.Aciklama
                });
                sayac++;
            }

            await _context.SaveChangesAsync();

            TempData["Bilgi"] = sayac > 0
                ? $"Geçen aydan {sayac} gider bu aya kopyalandı. Tutarı değişenleri düzenlemeyi unutmayın."
                : "Kopyalanacak gider bulunamadı. Geçen ay gider yok ya da bu ayın giderleri zaten girilmiş.";
            return RedirectToAction(nameof(Index));
        }
        // Filtrelenmiş giderleri Excel dosyası olarak indirir
        public async Task<IActionResult> ExcelAktar(int? isletmeId, int? kategoriId, int? yil, int? ay)
        {
            var sorgu = Giderlerim();
            if (isletmeId != null) sorgu = sorgu.Where(g => g.IsletmeId == isletmeId);
            if (kategoriId != null) sorgu = sorgu.Where(g => g.GiderKategorisi!.Id == kategoriId);
            if (yil != null) sorgu = sorgu.Where(g => g.Tarih.Year == yil);
            if (ay != null) sorgu = sorgu.Where(g => g.Tarih.Month == ay);

            var giderler = await sorgu
                .Include(g => g.Isletme)
                .Include(g => g.GiderKategorisi)
                .OrderByDescending(g => g.Tarih)
                .ToListAsync();

            using var kitap = new XLWorkbook();
            var sayfa = kitap.Worksheets.Add("Giderler");

            // Başlık satırı
            sayfa.Cell(1, 1).Value = "Tarih";
            sayfa.Cell(1, 2).Value = "İşletme";
            sayfa.Cell(1, 3).Value = "Kategori";
            sayfa.Cell(1, 4).Value = "Açıklama";
            sayfa.Cell(1, 5).Value = "Tutar";

            var baslik = sayfa.Range(1, 1, 1, 5);
            baslik.Style.Font.Bold = true;
            baslik.Style.Font.FontColor = XLColor.White;
            baslik.Style.Fill.BackgroundColor = XLColor.FromHtml("#dc3545");

            // Kayıtlar
            int satir = 2;
            foreach (var g in giderler)
            {
                sayfa.Cell(satir, 1).Value = g.Tarih;
                sayfa.Cell(satir, 2).Value = g.Isletme?.Ad ?? "";
                sayfa.Cell(satir, 3).Value = g.GiderKategorisi?.Ad ?? "";
                sayfa.Cell(satir, 4).Value = g.Aciklama ?? "";
                sayfa.Cell(satir, 5).Value = (double)g.Tutar;
                satir++;
            }

            // Toplam satırı
            sayfa.Cell(satir, 4).Value = "Toplam";
            sayfa.Cell(satir, 5).Value = (double)giderler.Sum(g => g.Tutar);
            sayfa.Range(satir, 4, satir, 5).Style.Font.Bold = true;

            // Biçimler
            sayfa.Column(1).Style.DateFormat.Format = "dd.MM.yyyy";
            sayfa.Column(5).Style.NumberFormat.Format = "#,##0.00 \"₺\"";
            sayfa.Columns().AdjustToContents();

            using var akis = new MemoryStream();
            kitap.SaveAs(akis);

            return File(akis.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Giderler_{DateTime.Today:yyyy-MM-dd}.xlsx");
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