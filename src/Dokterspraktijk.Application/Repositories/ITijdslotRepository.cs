using Dokterspraktijk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Repositories
{
    public interface ITijdslotRepository
    {
        Tijdslot? ZoekOpId(int id);
        Tijdslot? ZoekVoorDokterOpDatumEnTijd(int dokterId, DateOnly datum, TimeOnly tijd);
        List<Tijdslot> GeefTijdslotenVoorDokterOpDatum(int dokterId, DateOnly datum);
        void VoegToe(Tijdslot tijdslot);
        void WerkBij(Tijdslot tijdslot);
    }
}
