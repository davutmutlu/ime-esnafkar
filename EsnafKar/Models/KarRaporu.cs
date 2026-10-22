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

        public decimal OncekiGelir { get; set; }
        public decimal OncekiGider { get; set; }
        public decimal OncekiNetKar => OncekiGelir - OncekiGider;

        public List<KategoriToplami> GiderDagilimi { get; set; } = new();

        // İki rakam arasındaki değişimi yüzde olarak hesaplar
        public static decimal? Degisim(decimal simdi, decimal onceki)
        {
            if (onceki == 0) return null;
            return (simdi - onceki) / Math.Abs(onceki) * 100;
        }
    }

    public class KategoriToplami
    {
        public string Kategori { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
    }
}