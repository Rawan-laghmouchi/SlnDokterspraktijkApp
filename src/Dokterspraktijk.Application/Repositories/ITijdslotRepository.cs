using Dokterspraktijk.Domain.Entities;

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
