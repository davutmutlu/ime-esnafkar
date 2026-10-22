namespace EsnafKar.Models
{
    public static class KategoriSimgeleri
    {
        private static readonly Dictionary<string, string> Simgeler = new()
        {
            ["Kira"] = "bi-house-door",
            ["Maaş"] = "bi-people",
            ["Elektrik"] = "bi-lightning-charge",
            ["Su"] = "bi-droplet",
            ["Doğalgaz"] = "bi-fire",
            ["İnternet / Telefon"] = "bi-wifi",
            ["Vergi / Harç"] = "bi-bank",
            ["Bakım / Onarım"] = "bi-tools",
            ["Diğer"] = "bi-three-dots"
        };

        public static string Getir(string? kategoriAdi)
        {
            if (kategoriAdi != null && Simgeler.TryGetValue(kategoriAdi, out var simge))
            {
                return simge;
            }
            return "bi-tag";
        }
    }
}