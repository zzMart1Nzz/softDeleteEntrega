using System;

namespace softDeleteEntrega.Models
{
    public class Erreserba
    {
        public int Id { get; set; }
        public int Pista_id { get; set; }
        public int Bazkidea_id { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Ordua { get; set; }
    }
}
