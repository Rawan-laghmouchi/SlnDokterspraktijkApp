using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Infrastructure.Fakes
{
    public class FakeDoktersattestRepository : IDoktersattestRepository
    {
        private FakeDokterspraktijkDatastore _dataStore;
        public FakeDoktersattestRepository(FakeDokterspraktijkDatastore dataStore) {
            _dataStore = dataStore;
        }
        public Doktersattest? ZoekOpId(int id)
        {
            return _dataStore.Doktersattesten.FirstOrDefault(doktersattest => doktersattest.Id == id);
        }

        public Doktersattest? ZoekOpAfspraakId(int afspraakId)
        {
            return _dataStore.Doktersattesten.FirstOrDefault(doktersattest =>
                doktersattest.AfspraakId == afspraakId);
        }

        public void VoegToe(Doktersattest doktersattest)
        {
            _dataStore.Doktersattesten.Add(doktersattest);
        }

        public void WerkBij(Doktersattest doktersattest)
        {
            Doktersattest? bestaandDoktersattest = ZoekOpId(doktersattest.Id);

            if (bestaandDoktersattest == null)
            {
                _dataStore.Doktersattesten.Add(doktersattest);
            }
        }

    }
}
