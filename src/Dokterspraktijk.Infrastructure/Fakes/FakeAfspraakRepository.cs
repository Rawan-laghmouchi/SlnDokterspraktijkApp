using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;

namespace Dokterspraktijk.Infrastructure.Fakes
{
    public class FakeAfspraakRepository : IAfspraakRepository
    {
        private readonly FakeDokterspraktijkDatastore _dataStore;
        private readonly ITijdslotRepository _tijdslotRepository;

        public FakeAfspraakRepository(
            FakeDokterspraktijkDatastore dataStore,
            ITijdslotRepository tijdslotRepository)
        {
            _dataStore = dataStore;
            _tijdslotRepository = tijdslotRepository;
        }

        public List<Afspraak> GeefAfsprakenVoorPatient(int patientId)
        {
            return _dataStore.Afspraken
                .Where(afspraak => afspraak.PatientId == patientId)
                .ToList();
        }

        public void VoegToe(Afspraak afspraak)
        {
            _dataStore.Afspraken.Add(afspraak);
        }

        public void WerkBij(Afspraak afspraak)
        {
            Afspraak? bestaandeAfspraak = ZoekOpId(afspraak.Id);

            if (bestaandeAfspraak == null)
            {
                _dataStore.Afspraken.Add(afspraak);
            }
        }

        public Afspraak? ZoekOpId(int id)
        {
            return _dataStore.Afspraken.FirstOrDefault(afspraak => afspraak.Id == id);
        }

        public Afspraak? ZoekOpPatientDokterDatumEnTijd(
            int patientId,
            int dokterId,
            DateOnly datum,
            TimeOnly tijd)
        {
            return _dataStore.Afspraken.FirstOrDefault(afspraak =>
            {
                Tijdslot? tijdslot = _tijdslotRepository.ZoekOpId(afspraak.TijdslotId);

                return afspraak.PatientId == patientId &&
                       afspraak.DokterId == dokterId &&
                       tijdslot != null &&
                       tijdslot.Datum == datum &&
                       tijdslot.Tijd == tijd;
            });
        }
    }
}