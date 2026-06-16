using Dokterspraktijk.Domain.Entities;

namespace Dokterspraktijk.Application.Repositories
{
    public interface IAfspraakRepository
    {
        Afspraak? ZoekOpId(int id);
        Afspraak? ZoekOpPatientDokterDatumEnTijd(int patientId, int dokterId, DateOnly datum, TimeOnly tijd);
        List<Afspraak> GeefAfsprakenVoorPatient(int patientId);
        List<Afspraak> GeefAlleAfspraken();
        void VoegToe(Afspraak afspraak);
        void WerkBij(Afspraak afspraak);
    }
}
