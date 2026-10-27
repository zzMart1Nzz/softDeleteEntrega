using System.Collections.Generic;
using System.Threading.Tasks;
using softDeleteEntrega.Models;

namespace softDeleteEntrega.Services
{
    // Taldekideen zerrenden SELECT, INSERT eta DELETE kodearen konexio-puntuak.
    public interface ITaldearenZerbitzua
    {
        Task<IList<Erreserba>> LortuErreserbakAsync();
        Task<IList<Pista>> LortuPistakAsync(bool desgaituta);
        Task GehituErreserbaAsync(Erreserba erreserba);
        Task EzabatuPistaAsync(int pistaId);
    }
}
