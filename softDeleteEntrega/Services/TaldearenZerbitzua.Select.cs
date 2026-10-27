using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using softDeleteEntrega.Models;

namespace softDeleteEntrega.Services
{
    public partial class TaldearenZerbitzua
    {
        public Task<IList<Erreserba>> LortuErreserbakAsync()
        {
            // TODO (SELECT): ordezkatu throw zure APIaren benetako bidearekin:
            // return Api.LortuZerrendaAsync<Erreserba>("BENETAKO_ERRESERBEN_BIDEA");
            throw new NotImplementedException("Erreserben zerrenda oraindik ez dago erabilgarri.");
        }

        public Task<IList<Pista>> LortuPistakAsync(bool desgaituta)
        {
            // TODO (SELECT): zure endpoint-ak iragaziko du:
            // false -> desgaituta = 0; true -> desgaituta = 1.
            // return Api.LortuZerrendaAsync<Pista>("BENETAKO_PISTEN_BIDEA");
            throw new NotImplementedException("Pisten zerrenda oraindik ez dago erabilgarri.");
        }
    }
}
