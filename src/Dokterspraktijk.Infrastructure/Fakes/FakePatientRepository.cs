using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Infrastructure.Fakes
{
    public class FakePatientRepository : IPatientRepository
    {
        private readonly FakeDokterspraktijkDatastore _dataStore;
        public FakePatientRepository(FakeDokterspraktijkDatastore dataStore)
        {
            _dataStore = dataStore;
        }
        public void VoegToe(Patient patient)
        {
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
        public Patient? ZoekOpNaam(string naam)
        {
            return _dataStore.Patienten.FirstOrDefault(patient => 
                patient.Naam.Equals(naam, StringComparison.OrdinalIgnoreCase));
        }
    }
}
