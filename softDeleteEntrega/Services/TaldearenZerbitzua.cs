using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using softDeleteEntrega.Models;

namespace softDeleteEntrega.Services
{
    public class TaldearenZerbitzua : ITaldearenZerbitzua
    {
        public Task<IList<Erreserba>> LortuErreserbakAsync()
        {
            // TODO (SELECT): APIaren erreserba-zerrenda ekarri.
            throw new NotImplementedException("Erreserben zerrenda oraindik ez dago erabilgarri.");
        }

        public Task<IList<Pista>> LortuPistakAsync(bool desgaituta)
        {
            // TODO (SELECT): false -> desgaituta = 0; true -> desgaituta = 1.
            // Iragazkia taldekidearen SELECT funtzioak egingo du.
            throw new NotImplementedException("Pisten zerrenda oraindik ez dago erabilgarri.");
        }

        public Task GehituErreserbaAsync(Erreserba erreserba)
        {
            // TODO (INSERT): taldekidearen erreserba-sarrera konektatu.
            throw new NotImplementedException("Erreserbak gehitzeko aukera oraindik ez dago erabilgarri.");
        }

        public Task EzabatuPistaAsync(int pistaId)
        {
            // TODO (DELETE): desgaitutako pista behin betiko ezabatu.
            throw new NotImplementedException("Behin betiko ezabatzea oraindik ez dago erabilgarri.");
        }
    }
}
