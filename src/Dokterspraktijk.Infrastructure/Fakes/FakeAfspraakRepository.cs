using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;

namespace Dokterspraktijk.Infrastructure.Fakes
{
    public class FakeAfspraakRepository : IAfspraakRepository
    {
        private FakeDokterspraktijkDatastore _dataStore;
        private ITijdslotRepository _tijdslotRepository;

        public FakeAfspraakRepository(FakeDokterspraktijkDatastore dataStore, ITijdslotRepository tijdslotRepository)
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
            if (afspraak.Id == 0)
            {
                int nieuwId = 1;

                if (_dataStore.Afspraken.Any())
                {
                    nieuwId = _dataStore.Afspraken.Max(bestaandeAfspraak => bestaandeAfspraak.Id) + 1;
                }

                afspraak.Id = nieuwId;
            }

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
        public List<Afspraak> GeefAlleAfspraken()
        {
            return _dataStore.Afspraken.ToList();
        }
    }
}