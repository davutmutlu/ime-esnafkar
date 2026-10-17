namespace EsnafKar.Models
{
    public class KarRaporu
    {
        public int? IsletmeId { get; set; }
        public int Yil { get; set; }
        public int Ay { get; set; }

        public decimal ToplamGelir { get; set; }
        public decimal ToplamGider { get; set; }
        public decimal NetKar => ToplamGelir - ToplamGider;

        public List<KategoriToplami> GiderDagilimi { get; set; } = new();
    }

    public class KategoriToplami
    {
        public string Kategori { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
    }
}