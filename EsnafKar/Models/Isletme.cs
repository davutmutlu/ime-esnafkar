using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EsnafKar.Models
{
    public class Isletme
    {
        public int Id { get; set; }

        [Display(Name = "İşletme Adı")]
        [Required(ErrorMessage = "İşletme adı zorunludur.")]
        [StringLength(100, ErrorMessage = "İşletme adı en fazla 100 karakter olabilir.")]
        public string Ad { get; set; } = string.Empty;

        [Display(Name = "İşletme Türü")]
        [Required(ErrorMessage = "İşletme türü seçilmelidir.")]
        [StringLength(50, ErrorMessage = "İşletme türü en fazla 50 karakter olabilir.")]
        public string Tur { get; set; } = string.Empty;

        [Display(Name = "Kayıt Tarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;


        public string? KullaniciId { get; set; }
        public IdentityUser? Kullanici { get; set; }
    }
}
