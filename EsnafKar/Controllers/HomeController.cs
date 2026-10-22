using System.Diagnostics;
using System.Security.Claims;
using EsnafKar.Data;
using EsnafKar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EsnafKar.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return View();
            }

            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var baslangic = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var bitis = baslangic.AddMonths(1);

            var panel = new GostergePaneli
            {
                IsletmeSayisi = await _context.Isletmeler
                    .CountAsync(i => i.KullaniciId == kullaniciId),

                BuAyGelir = await _context.Gelirler
                    .Where(g => g.Isletme!.KullaniciId == kullaniciId && g.Tarih >= baslangic && g.Tarih < bitis)
                    .SumAsync(g => g.Tutar),

                BuAyGider = await _context.Giderler
                    .Where(g => g.Isletme!.KullaniciId == kullaniciId && g.Tarih >= baslangic && g.Tarih < bitis)
                    .SumAsync(g => g.Tutar)
            };

            return View(panel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}