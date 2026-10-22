namespace EsnafKar.Models
{
    public class GostergePaneli
    {
        public int IsletmeSayisi { get; set; }
        public decimal BuAyGelir { get; set; }
        public decimal BuAyGider { get; set; }
        public decimal BuAyNetKar => BuAyGelir - BuAyGider;
    }
}