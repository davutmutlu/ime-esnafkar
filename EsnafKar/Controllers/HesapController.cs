using EsnafKar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EsnafKar.Controllers
{

    [AllowAnonymous]
    public class HesapController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HesapController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Kayıt formunu açar
        public IActionResult Kayit()
        {
            return View();
        }

        // Kayıt formu gönderilince çalışır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kayit(KayitModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var kullanici = new IdentityUser
            {
                UserName = model.Eposta,
                Email = model.Eposta
            };

            var sonuc = await _userManager.CreateAsync(kullanici, model.Sifre);

            if (sonuc.Succeeded)
            {
                await _signInManager.SignInAsync(kullanici, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var hata in sonuc.Errors.Select(h => h.Description).Distinct())
            {
                ModelState.AddModelError(string.Empty, hata);
            }
            return View(model);
        }

        // Giriş formunu açar
        public IActionResult Giris(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Giriş formu gönderilince çalışır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Giris(GirisModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sonuc = await _signInManager.PasswordSignInAsync(
                model.Eposta, model.Sifre, model.BeniHatirla, lockoutOnFailure: true);

            if (sonuc.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            if (sonuc.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Çok fazla hatalı deneme yapıldı. Lütfen birkaç dakika sonra tekrar deneyin.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
            }
            return View(model);
        }

        // Çıkış yapar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cikis()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}