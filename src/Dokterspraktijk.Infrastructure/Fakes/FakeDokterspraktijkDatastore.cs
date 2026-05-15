using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;

namespace Dokterspraktijk.Infrastructure.Fakes
{
    public class FakeDokterspraktijkDatastore
    {
        public List<Dokter> Dokters { get; }
        public List<Patient> Patienten { get; }
        public List<Tijdslot> Tijdsloten { get; }
        public List<Afspraak> Afspraken { get; }
        public List<Doktersattest> Doktersattesten { get; }

        public FakeDokterspraktijkDatastore()
        {
            Dokters = new List<Dokter>();
            Patienten = new List<Patient>();
            Tijdsloten = new List<Tijdslot>();
            Afspraken = new List<Afspraak>();
            Doktersattesten = new List<Doktersattest>();

            Seed();
        }

        private void Seed()
        {
            Dokters.Add(new Dokter(1, "Timmermans", "Huisarts"));
            Dokters.Add(new Dokter(2, "Brancaert", "Huisarts"));

            Patienten.Add(new Patient(1, "Rawan"));
            Patienten.Add(new Patient(2, "Hans"));

            Tijdsloten.Add(new Tijdslot(
                1,
                1,
                new DateOnly(2026, 6, 19),
                new TimeOnly(9, 30),
                TijdslotStatus.NietBeschikbaar));

            Tijdsloten.Add(new Tijdslot(
                2,
                2,
                new DateOnly(2026, 6, 19),
                new TimeOnly(10, 0),
                TijdslotStatus.NietBeschikbaar));

            Tijdsloten.Add(new Tijdslot(
                3,
                1,
                new DateOnly(2026, 6, 19),
                new TimeOnly(10, 0),
                TijdslotStatus.Beschikbaar));
        }
    }
}