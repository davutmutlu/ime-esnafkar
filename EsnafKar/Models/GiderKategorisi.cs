using System.ComponentModel.DataAnnotations;

namespace EsnafKar.Models
{
    public class GiderKategorisi
    {
        public int Id { get; set; }

        [Display(Name = "Kategori Adı")]
        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Kategori adı en fazla 50 karakter olabilir.")]
        public string Ad { get; set; } = string.Empty;
    }
}