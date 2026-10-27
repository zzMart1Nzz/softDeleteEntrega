namespace softDeleteEntrega.Models
{
    public class Erabiltzailea
    {
        public int Id { get; set; }
        public string Izena { get; set; }
        public string Abizena { get; set; }
        public string IzenOsoa => ((Izena ?? "") + " " + (Abizena ?? "")).Trim();
    }
}
