using System.ComponentModel.DataAnnotations;

namespace EsnafKar.Models
{
    public class KayitModel
    {
        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Eposta { get; set; } = string.Empty;

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        public string Sifre { get; set; } = string.Empty;

        [Display(Name = "Şifre (Tekrar)")]
        [Required(ErrorMessage = "Şifreyi tekrar girin.")]
        [DataType(DataType.Password)]
        [Compare("Sifre", ErrorMessage = "Şifreler aynı değil.")]
        public string SifreTekrar { get; set; } = string.Empty;
    }

    public class GirisModel
    {
        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Eposta { get; set; } = string.Empty;

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Sifre { get; set; } = string.Empty;

        [Display(Name = "Beni hatırla")]
        public bool BeniHatirla { get; set; }
    }
}