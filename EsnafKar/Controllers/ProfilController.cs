using System.Security.Claims;
using EsnafKar.Data;
using EsnafKar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EsnafKar.Controllers
{
    public class ProfilController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ProfilController(AppDbContext context,
                                UserManager<IdentityUser> userManager,
                                SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Profil sayfasındaki hesap özetini hazırlar
        private async Task<ProfilBilgisi> ProfilHazirla()
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return new ProfilBilgisi
            {
                Eposta = User.Identity?.Name ?? "",
                IsletmeSayisi = await _context.Isletmeler.CountAsync(i => i.KullaniciId == kullaniciId),
                GelirKaydi = await _context.Gelirler.CountAsync(g => g.Isletme!.KullaniciId == kullaniciId),
                GiderKaydi = await _context.Giderler.CountAsync(g => g.Isletme!.KullaniciId == kullaniciId)
            };
        }

        // Profil sayfasını açar
        public async Task<IActionResult> Index()
        {
            return View(await ProfilHazirla());
        }

        // Şifre değiştirme formu gönderilince çalışır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SifreDegistir([Bind(Prefix = "Sifre")] SifreDegistirModel model)
        {
            if (ModelState.IsValid)
            {
                var kullanici = await _userManager.GetUserAsync(User);
                if (kullanici == null) return RedirectToAction("Giris", "Hesap");

                var sonuc = await _userManager.ChangePasswordAsync(kullanici, model.MevcutSifre, model.YeniSifre);

                if (sonuc.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(kullanici);
                    TempData["Bilgi"] = "Şifreniz başarıyla değiştirildi.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var hata in sonuc.Errors)
                {
                    ModelState.AddModelError(string.Empty, hata.Description);
                }
            }

            // Hata varsa sayfayı yazılanlarla birlikte tekrar göster
            var bilgi = await ProfilHazirla();
            bilgi.Sifre = model;
            return View("Index", bilgi);
        }
    }
}