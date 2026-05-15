using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Infrastructure.Fakes
{
    public class FakeAfspraakRepository : IAfspraakRepository
    {
        private readonly FakeDokterspraktijkDatastore _dataStore;
        private readonly FakeTijdslotRepository _tijdslotRepo;

        public FakeAfspraakRepository(FakeDokterspraktijkDatastore dataStore, FakeTijdslotRepository tijdslotRepo)
        {
            _dataStore = dataStore;
            _tijdslotRepo = tijdslotRepo;
        }
        public List<Afspraak> GeefAfsprakenVoorPatient(int patientId)
        {
            return _dataStore.Afspraken
                .Where(afspraak => 
                afspraak.Id == patientId)
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

        public Afspraak? ZoekOpPatientDokterDatumEnTijd(int patientId, int dokterId, DateOnly datum, TimeOnly tijd)
        {
            return _dataStore.Afspraken.FirstOrDefault(afspraak =>
            {
                Tijdslot? tijdslot = _tijdslotRepo.ZoekOpId(afspraak.TijdslotId);

                return afspraak.PatientId == patientId &&
                       afspraak.DokterId == dokterId &&
                       tijdslot != null &&
                       tijdslot.Datum == datum &&
                       tijdslot.Tijd == tijd;
            });
        }
    }
}
