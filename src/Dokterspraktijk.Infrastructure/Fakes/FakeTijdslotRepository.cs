using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Infrastructure.Fakes
{
    public class FakeTijdslotRepository : ITijdslotRepository
    {
        private readonly FakeDokterspraktijkDatastore _dataStore;
        public FakeTijdslotRepository(FakeDokterspraktijkDatastore dataStore)
        {
            _dataStore = dataStore;
        }

        public List<Tijdslot> GeefTijdslotenVoorDokterOpDatum(int dokterId, DateOnly datum)
        {
            return _dataStore.Tijdsloten
                .Where(tijdslot =>
                tijdslot.DokterId == dokterId &&
                tijdslot.Datum == datum)
                .OrderBy(tijdslot => tijdslot.Tijd)
                .ToList();
        }

        public void VoegToe(Tijdslot tijdslot)
        {
            if (tijdslot.Id == 0)
            {
                int nieuwId = 1;

                if (_dataStore.Tijdsloten.Any())
                {
                    nieuwId = _dataStore.Tijdsloten.Max(bestaandTijdslot => bestaandTijdslot.Id) + 1;
                }

                tijdslot.StelIdIn(nieuwId);
            }

            _dataStore.Tijdsloten.Add(tijdslot);
        }

        public void WerkBij(Tijdslot tijdslot)
        {
            Tijdslot? bestaandTijdslot = ZoekOpId(tijdslot.Id);
            if (bestaandTijdslot == null)
            {
                _dataStore.Tijdsloten.Add(tijdslot);
            }
        }

        public Tijdslot? ZoekOpId(int id)
        {
            return _dataStore.Tijdsloten.FirstOrDefault(tijdslot => tijdslot.Id == id );
        }

        public Tijdslot? ZoekVoorDokterOpDatumEnTijd(int dokterId, DateOnly datum, TimeOnly tijd)
        {
            return _dataStore.Tijdsloten.FirstOrDefault(tijdslot =>
            tijdslot.DokterId == dokterId &&
            tijdslot.Datum == datum &&
            tijdslot.Tijd == tijd);
        }
    }
}
