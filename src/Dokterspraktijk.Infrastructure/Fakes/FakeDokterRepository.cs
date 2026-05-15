using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Infrastructure.Fakes
{
    public class FakeDokterRepository : IDokterRepository
    {
        private readonly FakeDokterspraktijkDatastore _dataStore;
        public FakeDokterRepository(FakeDokterspraktijkDatastore dataStore)
        {
            _dataStore = dataStore;
        }

        public List<Dokter> GeefAlleDokters()
        {
            return _dataStore.Dokters.ToList();
        }

        public void VoegToe(Dokter dokter)
        {
            _dataStore.Dokters.Add(dokter);
        }

        public Dokter? ZoekOpId(int id)
        {
            return _dataStore.Dokters.FirstOrDefault(dokter => dokter.Id == id);
        }

        public Dokter? ZoekOpNaam(string naam)
        {
            return _dataStore.Dokters.FirstOrDefault(dokter =>
                dokter.Naam.Equals(naam, StringComparison.OrdinalIgnoreCase)); // nakijken
        }
    }
}
