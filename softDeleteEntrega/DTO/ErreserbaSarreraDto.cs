using System;
using System.Globalization;
using softDeleteEntrega.Models;

namespace softDeleteEntrega.DTO
{
    // APIaren DateOnly eta TimeOnly eremuek ISO testua behar dute.
    public class ErreserbaSarreraDto
    {
        public int Pista_id { get; set; }
        public int Bazkidea_id { get; set; }
        public string Data { get; set; }
        public string Ordua { get; set; }

        public static ErreserbaSarreraDto Sortu(Erreserba erreserba)
        {
            if (erreserba == null) throw new ArgumentNullException(nameof(erreserba));
            return new ErreserbaSarreraDto
            {
                Pista_id = erreserba.Pista_id,
                Bazkidea_id = erreserba.Bazkidea_id,
                Data = erreserba.Data.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Ordua = erreserba.Ordua.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture)
            };
        }
    }
}
