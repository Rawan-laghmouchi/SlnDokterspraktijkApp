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
    public class KomendeAfsprakenBekijkenStepDefinitions
    {
        private IAfspraakService _afspraakService;
        private IPatientRepository _patientRepository;
        private IDokterRepository _dokterRepository;
        private ITijdslotRepository _tijdslotRepository;
        private IAfspraakRepository _afspraakRepository;
        private DokterspraktijkScenarioContext _context;

        private string _huidigePatientVoornaam;
        private string _huidigePatientAchternaam;

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

            _huidigePatientVoornaam = string.Empty;
            _huidigePatientAchternaam = string.Empty;
        }

        [Given(@"patiënt (.*) (.*) heeft de volgende afspraken:")]
        public void GivenPatientHeeftDeVolgendeAfspraken(
            string patientVoornaam,
            string patientAchternaam,
            Table table)
        {
            Patient patient = ZorgDatPatientBestaat(patientVoornaam, patientAchternaam);

            _huidigePatientVoornaam = patient.Voornaam;
            _huidigePatientAchternaam = patient.Achternaam;

            foreach (var rij in table.Rows)
            {
                DateOnly datum = ParseDatum(rij["Datum"]);
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
                    patient.Voornaam,
                    patient.Achternaam,
                    patient.Email,
                    patient.Telefoonnummer,
                    patient.Rijksregisternummer,
                    dokter.Naam,
                    datum,
                    tijd,
                    reden);

                Assert.True(resultaat.IsGelukt, resultaat.Melding);
            }
        }

        [Given(@"patiënt (.*) (.*) heeft volgende afspraken:")]
        public void GivenPatientHeeftVolgendeAfspraken(
            string patientVoornaam,
            string patientAchternaam,
            Table table)
        {
            GivenPatientHeeftDeVolgendeAfspraken(
                patientVoornaam,
                patientAchternaam,
                table);
        }

        [When(@"zij haar komende afspraken raadpleegt")]
        public void WhenZijHaarKomendeAfsprakenRaadpleegt()
        {
            Assert.False(string.IsNullOrWhiteSpace(_huidigePatientVoornaam));
            Assert.False(string.IsNullOrWhiteSpace(_huidigePatientAchternaam));

            _context.GeraadpleegdeAfspraken = _afspraakService.GeefKomendeAfspraken(
                _huidigePatientVoornaam,
                _huidigePatientAchternaam,
                new DateOnly(2026, 1, 1));
        }

        [When(@"patiënt (.*) (.*) haar komende afspraken raadpleegt")]
        public void WhenPatientHaarKomendeAfsprakenRaadpleegt(
            string patientVoornaam,
            string patientAchternaam)
        {
            Patient patient = ZorgDatPatientBestaat(patientVoornaam, patientAchternaam);

            _huidigePatientVoornaam = patient.Voornaam;
            _huidigePatientAchternaam = patient.Achternaam;

            _context.GeraadpleegdeAfspraken = _afspraakService.GeefKomendeAfspraken(
                patient.Voornaam,
                patient.Achternaam,
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
            Assert.False(string.IsNullOrWhiteSpace(_huidigePatientVoornaam));
            Assert.False(string.IsNullOrWhiteSpace(_huidigePatientAchternaam));

            Assert.All(_context.GeraadpleegdeAfspraken, afspraak =>
            {
                Assert.Equal(_huidigePatientVoornaam, afspraak.PatientVoornaam);
                Assert.Equal(_huidigePatientAchternaam, afspraak.PatientAchternaam);
            });
        }

        private Dokter ZorgDatDokterBestaat(string dokterNaam)
        {
            Dokter? dokter = _dokterRepository.ZoekOpNaam(dokterNaam);

            Assert.NotNull(dokter);

            return dokter;
        }

        private Patient ZorgDatPatientBestaat(string patientVoornaam, string patientAchternaam)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientVoornaam, patientAchternaam);

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
                bestaandTijdslot.Status = status;
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