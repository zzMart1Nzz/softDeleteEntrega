using System;
using System.Threading.Tasks;

namespace softDeleteEntrega.Services
{
    public partial class TaldearenZerbitzua
    {
        public Task EzabatuPistaAsync(int pistaId)
        {
            // TODO (DELETE): desgaitutako pista behin betiko ezabatzeko:
            // return Api.EzabatuAsync("BENETAKO_DELETE_BIDEA/" + pistaId);
            // Aktiboen Ezabatu botoiak soft delete deitzen jarraitzen du.
            throw new NotImplementedException("Behin betiko ezabatzea oraindik ez dago erabilgarri.");
        }
    }
}
