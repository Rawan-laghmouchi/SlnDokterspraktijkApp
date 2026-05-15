using System.Globalization;
using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;
using Dokterspraktijk.Tests.BDD.Support;
using Reqnroll;
using Xunit;

namespace Dokterspraktijk.Tests.BDD.StepDefinitions
{
    [Binding]
    [Scope(Feature = "Komende afspraken bekijken")]
    public class KomendeAfsprakenBekijkenStepDefinitions
    {
        private readonly IAfspraakService _afspraakService;
        private readonly IPatientRepository _patientRepository;
        private readonly IDokterRepository _dokterRepository;
        private readonly ITijdslotRepository _tijdslotRepository;
        private readonly IAfspraakRepository _afspraakRepository;
        private readonly DokterspraktijkScenarioContext _context;

        private string _huidigePatientNaam;

        public KomendeAfsprakenBekijkenStepDefinitions(
            IAfspraakService afspraakService,
            IPatientRepository patientRepository,
            IDokterRepository dokterRepository,
            ITijdslotRepository tijdslotRepository,
            IAfspraakRepository afspraakRepository,
            DokterspraktijkScenarioContext context)
        {
            _afspraakService = afspraakService;
            _patientRepository = patientRepository;
            _dokterRepository = dokterRepository;
            _tijdslotRepository = tijdslotRepository;
            _afspraakRepository = afspraakRepository;
            _context = context;
            _huidigePatientNaam = string.Empty;
        }

        [Given(@"patiënt (.*) heeft de volgende afspraken:")]
        public void GivenPatientHeeftDeVolgendeAfspraken(string patientNaam, Table table)
        {
            Patient patient = ZorgDatPatientBestaat(patientNaam);
            _huidigePatientNaam = patient.Naam;

            foreach (var rij in table.Rows)
            {
                DateOnly datum = ParseDatum(rij["Datum"]); // veranderen naargelang tutorial
                TimeOnly tijd = ParseTijd(rij["Tijd"]);
                string dokterNaam = rij["Dokter"];
                string reden = rij["Reden"];

                Dokter dokter = ZorgDatDokterBestaat(dokterNaam);

                ZorgDatTijdslotBestaat(
                    dokter.Id,
                    datum,
                    tijd,
                    TijdslotStatus.Beschikbaar);

                ResultaatDto resultaat = _afspraakService.MaakAfspraak(
                    patient.Naam,
                    dokter.Naam,
                    datum,
                    tijd,
                    reden);

                Assert.True(resultaat.IsGelukt, resultaat.Melding);
            }
        }

        [Given(@"patiënt (.*) heeft volgende afspraken:")]
        public void GivenPatientHeeftVolgendeAfspraken(string patientNaam, Table table)
        {
            GivenPatientHeeftDeVolgendeAfspraken(patientNaam, table);
        }

        [When(@"zij haar komende afspraken raadpleegt")]
        public void WhenZijHaarKomendeAfsprakenRaadpleegt()
        {
            Assert.False(string.IsNullOrWhiteSpace(_huidigePatientNaam));

            _context.GeraadpleegdeAfspraken = _afspraakService.GeefKomendeAfspraken(
                _huidigePatientNaam,
                new DateOnly(2026, 1, 1));
        }

        [When(@"patiënt (.*) haar komende afspraken raadpleegt")]
        public void WhenPatientHaarKomendeAfsprakenRaadpleegt(string patientNaam)
        {
            ZorgDatPatientBestaat(patientNaam);

            _huidigePatientNaam = patientNaam;

            _context.GeraadpleegdeAfspraken = _afspraakService.GeefKomendeAfspraken(
                patientNaam,
                new DateOnly(2026, 1, 1));
        }

        [Then(@"ziet zij de volgende afspraken:")]
        public void ThenZietZijDeVolgendeAfspraken(Table table)
        {
            Assert.Equal(table.RowCount, _context.GeraadpleegdeAfspraken.Count);

            foreach (var rij in table.Rows)
            {
                DateOnly verwachteDatum = ParseDatum(rij["Datum"]);
                TimeOnly verwachteTijd = ParseTijd(rij["Tijd"]);
                string verwachteDokterNaam = rij["Dokter"];
                string verwachteReden = rij["Reden"];

                AfspraakDto? gevondenAfspraak = _context.GeraadpleegdeAfspraken.FirstOrDefault(afspraak =>
                    afspraak.Datum == verwachteDatum &&
                    afspraak.Tijd == verwachteTijd &&
                    afspraak.DokterNaam == verwachteDokterNaam &&
                    afspraak.Reden == verwachteReden);

                Assert.NotNull(gevondenAfspraak);
            }
        }

        [Then(@"ziet zij enkel haar eigen afspraken")]
        public void ThenZietZijEnkelHaarEigenAfspraken()
        {
            Assert.False(string.IsNullOrWhiteSpace(_huidigePatientNaam));

            Assert.All(_context.GeraadpleegdeAfspraken, afspraak =>
            {
                Assert.Equal(_huidigePatientNaam, afspraak.PatientNaam);
            });
        }
        private Dokter ZorgDatDokterBestaat(string dokterNaam)
        {
            Dokter? dokter = _dokterRepository.ZoekOpNaam(dokterNaam);

            Assert.NotNull(dokter);

            return dokter;
        }

        private Patient ZorgDatPatientBestaat(string patientNaam)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientNaam);

            Assert.NotNull(patient);

            return patient;
        }

        private Tijdslot ZorgDatTijdslotBestaat(
            int dokterId,
            DateOnly datum,
            TimeOnly tijd,
            TijdslotStatus status)
        {
            Tijdslot? bestaandTijdslot = _tijdslotRepository.ZoekVoorDokterOpDatumEnTijd(
                dokterId,
                datum,
                tijd);

            if (bestaandTijdslot != null)
            {
                if (status == TijdslotStatus.Beschikbaar)
                {
                    bestaandTijdslot.MaakBeschikbaar();
                }

                if (status == TijdslotStatus.NietBeschikbaar)
                {
                    bestaandTijdslot.MaakNietBeschikbaar();
                }

                _tijdslotRepository.WerkBij(bestaandTijdslot);

                return bestaandTijdslot;
            }

            Tijdslot tijdslot = new Tijdslot(
                0,
                dokterId,
                datum,
                tijd,
                status);

            _tijdslotRepository.VoegToe(tijdslot);

            return tijdslot;
        }

        private static DateOnly ParseDatum(string datumTekst)
        {
            return DateOnly.ParseExact(
                datumTekst,
                "dd-MM-yyyy",
                new CultureInfo("nl-BE"),
                DateTimeStyles.None);
        }

        private static TimeOnly ParseTijd(string tijdTekst)
        {
            return TimeOnly.ParseExact(
                tijdTekst,
                "HH:mm",
                new CultureInfo("nl-BE"),
                DateTimeStyles.None);
        }
    }
}