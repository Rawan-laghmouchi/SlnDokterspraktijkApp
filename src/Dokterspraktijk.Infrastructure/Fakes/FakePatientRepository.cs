using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;

namespace Dokterspraktijk.Infrastructure.Fakes
{
    public class FakePatientRepository : IPatientRepository
    {
        private FakeDokterspraktijkDatastore _dataStore;

        public FakePatientRepository(FakeDokterspraktijkDatastore dataStore)
        {
            _dataStore = dataStore;
        }

        public void VoegToe(Patient patient)
        {
            if (patient.Id == 0)
            {
                int nieuwId = 1;

                if (_dataStore.Patienten.Any())
                {
                    nieuwId = _dataStore.Patienten.Max(bestaandePatient => bestaandePatient.Id) + 1;
                }

                patient.Id = nieuwId;
            }

            _dataStore.Patienten.Add(patient);
        }

        public void WerkBij(Patient patient)
        {
            Patient? bestaandePatient = ZoekOpId(patient.Id);

            if (bestaandePatient == null)
            {
                _dataStore.Patienten.Add(patient);
            }
        }

        public Patient? ZoekOpId(int id)
        {
            return _dataStore.Patienten.FirstOrDefault(patient => patient.Id == id);
        }

        public Patient? ZoekOpNaam(string voornaam, string achternaam)
        {
            return _dataStore.Patienten.FirstOrDefault(patient =>
                patient.Voornaam.Equals(voornaam, StringComparison.OrdinalIgnoreCase) &&
                patient.Achternaam.Equals(achternaam, StringComparison.OrdinalIgnoreCase));
        }
    }
}