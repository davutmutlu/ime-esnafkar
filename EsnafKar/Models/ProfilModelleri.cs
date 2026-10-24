using System.ComponentModel.DataAnnotations;

namespace EsnafKar.Models
{
    // Şifre değiştirme formunun alanları
    public class SifreDegistirModel
    {
        [Required(ErrorMessage = "Mevcut şifrenizi girin.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string MevcutSifre { get; set; } = "";

        [Required(ErrorMessage = "Yeni şifreyi girin.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalı.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string YeniSifre { get; set; } = "";

        [Required(ErrorMessage = "Yeni şifreyi tekrar girin.")]
        [Compare("YeniSifre", ErrorMessage = "Şifreler aynı değil.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre (Tekrar)")]
        public string YeniSifreTekrar { get; set; } = "";
    }

    // Profil sayfasında gösterilecek bilgiler
    public class ProfilBilgisi
    {
        public string Eposta { get; set; } = "";
        public int IsletmeSayisi { get; set; }
        public int GelirKaydi { get; set; }
        public int GiderKaydi { get; set; }
        public SifreDegistirModel Sifre { get; set; } = new();
    }
}