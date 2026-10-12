using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsnafKar.Models
{
    public class Gider
    {
        public int Id { get; set; }

        [Display(Name = "İşletme")]
        [Required(ErrorMessage = "İşletme seçilmelidir.")]
        public int IsletmeId { get; set; }
        public Isletme? Isletme { get; set; }

        [Display(Name = "Kategori")]
        [Required(ErrorMessage = "Kategori seçilmelidir.")]
        public int GiderKategorisiId { get; set; }
        public GiderKategorisi? GiderKategorisi { get; set; }

        [Display(Name = "Tutar (₺)")]
        [Range(0.01, 100000000, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Tutar { get; set; }

        [Display(Name = "Tarih")]
        [DataType(DataType.Date)]
        public DateTime Tarih { get; set; } = DateTime.Today;

        [Display(Name = "Açıklama")]
        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
        public string? Aciklama { get; set; }
    }
}