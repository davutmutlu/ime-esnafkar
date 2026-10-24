using System.Security.Claims;
using ClosedXML.Excel;
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

        // Filtreleri sorguya uygular (liste ve Excel aynı filtreyi kullanır)
        private static IQueryable<Gelir> FiltreUygula(IQueryable<Gelir> sorgu, int? isletmeId, int? yil, int? ay)
        {
            if (isletmeId != null) sorgu = sorgu.Where(g => g.IsletmeId == isletmeId);
            if (yil != null) sorgu = sorgu.Where(g => g.Tarih.Year == yil);
            if (ay != null) sorgu = sorgu.Where(g => g.Tarih.Month == ay);
            return sorgu;
        }

        // Liste sayfası (filtreli)
        public async Task<IActionResult> Index(int? isletmeId, int? yil, int? ay)
        {
            var gelirler = await FiltreUygula(Gelirlerim(), isletmeId, yil, ay)
                .Include(g => g.Isletme)
                .OrderByDescending(g => g.Tarih)
                .ToListAsync();

            IsletmeListesiniHazirla(isletmeId);
            ViewData["SeciliYil"] = yil;
            ViewData["SeciliAy"] = ay;
            return View(gelirler);
        }

        // Filtrelenmiş listeyi Excel dosyası olarak indirir
        public async Task<IActionResult> ExcelAktar(int? isletmeId, int? yil, int? ay)
        {
            var gelirler = await FiltreUygula(Gelirlerim(), isletmeId, yil, ay)
                .Include(g => g.Isletme)
                .OrderByDescending(g => g.Tarih)
                .ToListAsync();

            using var kitap = new XLWorkbook();
            var sayfa = kitap.Worksheets.Add("Gelirler");

            // Başlık satırı
            sayfa.Cell(1, 1).Value = "Tarih";
            sayfa.Cell(1, 2).Value = "İşletme";
            sayfa.Cell(1, 3).Value = "Açıklama";
            sayfa.Cell(1, 4).Value = "Günlük Ciro";

            var baslik = sayfa.Range(1, 1, 1, 4);
            baslik.Style.Font.Bold = true;
            baslik.Style.Font.FontColor = XLColor.White;
            baslik.Style.Fill.BackgroundColor = XLColor.FromHtml("#198754");

            // Kayıtlar
            int satir = 2;
            foreach (var g in gelirler)
            {
                sayfa.Cell(satir, 1).Value = g.Tarih;
                sayfa.Cell(satir, 2).Value = g.Isletme?.Ad ?? "";
                sayfa.Cell(satir, 3).Value = g.Aciklama ?? "";
                sayfa.Cell(satir, 4).Value = (double)g.Tutar;
                satir++;
            }

            // Toplam satırı
            sayfa.Cell(satir, 3).Value = "Toplam";
            sayfa.Cell(satir, 4).Value = (double)gelirler.Sum(g => g.Tutar);
            sayfa.Range(satir, 3, satir, 4).Style.Font.Bold = true;

            // Biçimler
            sayfa.Column(1).Style.DateFormat.Format = "dd.MM.yyyy";
            sayfa.Column(4).Style.NumberFormat.Format = "#,##0.00 \"₺\"";
            sayfa.Columns().AdjustToContents();

            using var akis = new MemoryStream();
            kitap.SaveAs(akis);

            return File(akis.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Gelirler_{DateTime.Today:yyyy-MM-dd}.xlsx");
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
