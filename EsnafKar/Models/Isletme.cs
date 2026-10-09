using System.ComponentModel.DataAnnotations;

namespace EsnafKar.Models
{
    public class Isletme
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İşletme adı zorunludur.")]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Tur { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    }
}