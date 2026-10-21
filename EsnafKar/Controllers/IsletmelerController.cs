using System.Security.Claims;
using EsnafKar.Data;
using EsnafKar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EsnafKar.Controllers
{
    public class IsletmelerController : Controller
    {
        private readonly AppDbContext _context;

        public IsletmelerController(AppDbContext context)
        {
            _context = context;
        }

        // Giriş yapan kullanıcının numarası
        private string KullaniciId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Sadece giriş yapan kullanıcının işletmeleri
        private IQueryable<Isletme> Isletmelerim()
        {
            var kullaniciId = KullaniciId;
            return _context.Isletmeler.Where(i => i.KullaniciId == kullaniciId);
        }

        // Liste
        public async Task<IActionResult> Index()
        {
            var isletmeler = await Isletmelerim().OrderBy(i => i.Ad).ToListAsync();
            return View(isletmeler);
        }

        // Detay
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var isletme = await Isletmelerim().FirstOrDefaultAsync(i => i.Id == id);
            if (isletme == null) return NotFound();

            return View(isletme);
        }

        // Ekleme formunu açar
        public IActionResult Create()
        {
            return View();
        }

        // Ekleme formu gönderilince çalışır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ad,Tur")] Isletme isletme)
        {
            if (!ModelState.IsValid) return View(isletme);

            isletme.KullaniciId = KullaniciId;
            _context.Add(isletme);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Düzenleme formunu açar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var isletme = await Isletmelerim().FirstOrDefaultAsync(i => i.Id == id);
            if (isletme == null) return NotFound();

            return View(isletme);
        }

        // Düzenleme formu gönderilince çalışır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Tur")] Isletme isletme)
        {
            if (id != isletme.Id) return NotFound();
            if (!ModelState.IsValid) return View(isletme);

            var kayit = await Isletmelerim().FirstOrDefaultAsync(i => i.Id == id);
            if (kayit == null) return NotFound();

            kayit.Ad = isletme.Ad;
            kayit.Tur = isletme.Tur;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Silme onay sayfasını açar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var isletme = await Isletmelerim().FirstOrDefaultAsync(i => i.Id == id);
            if (isletme == null) return NotFound();

            return View(isletme);
        }

        // Silme onaylanınca çalışır
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var isletme = await Isletmelerim().FirstOrDefaultAsync(i => i.Id == id);
            if (isletme != null)
            {
                _context.Isletmeler.Remove(isletme);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}