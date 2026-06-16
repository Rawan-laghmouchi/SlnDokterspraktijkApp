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
        public List<AfspraakCategorie> AfspraakCategorieen { get; }

        public FakeDokterspraktijkDatastore()
        {
            Dokters = new List<Dokter>();
            Patienten = new List<Patient>();
            Tijdsloten = new List<Tijdslot>();
            Afspraken = new List<Afspraak>();
            Doktersattesten = new List<Doktersattest>();
            AfspraakCategorieen = new List<AfspraakCategorie>();

            Seed();
        }

        private void Seed()
        {
            Dokters.Add(new Dokter(1, "Timmermans", "Huisarts"));
            Dokters.Add(new Dokter(2, "Brancaert", "Huisarts"));

            Patient sara = new Patient(1, "Sara", "Peeters");
            sara.Email = "sara.peeters@gmail.be";
            sara.Telefoonnummer = "0470112233";
            sara.Rijksregisternummer = "22.22.22-222.22";
            Patienten.Add(sara);

            Patient hans = new Patient(2, "Hans", "Vandenbogaerde");
            hans.Email = "hans.vandenbogaerde@gmail.be";
            hans.Telefoonnummer = "0470654321";
            hans.Rijksregisternummer = "11.11.11-111.11";
            Patienten.Add(hans);

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

            AfspraakCategorieen.Add(new AfspraakCategorie
            {
                Id = 1,
                Naam = "Consultatie"
            });

            AfspraakCategorieen.Add(new AfspraakCategorie
            {
                Id = 2,
                Naam = "Controle"
            });

            AfspraakCategorieen.Add(new AfspraakCategorie
            {
                Id = 3,
                Naam = "Bloedafname"
            });
        }
    }
}