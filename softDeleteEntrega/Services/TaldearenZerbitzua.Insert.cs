using System;
using System.Threading.Tasks;
using softDeleteEntrega.DTO;
using softDeleteEntrega.Models;

namespace softDeleteEntrega.Services
{
    public partial class TaldearenZerbitzua
    {
        public Task GehituErreserbaAsync(Erreserba erreserba)
        {
            // TODO (INSERT): ordezkatu throw zure APIaren benetako bidearekin:
            // return Api.SortuAsync<Erreserba>("BENETAKO_INSERT_BIDEA", ErreserbaSarreraDto.Sortu(erreserba));
            // DTOak data eta ordua APIaren formatura bihurtzen ditu.
            throw new NotImplementedException("Erreserbak gehitzeko aukera oraindik ez dago erabilgarri.");
        }
    }
}
