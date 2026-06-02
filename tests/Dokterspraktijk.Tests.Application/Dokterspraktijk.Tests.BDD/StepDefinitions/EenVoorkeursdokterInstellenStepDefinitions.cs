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
    [Scope(Feature = "Een voorkeursdokter instellen")]
    public class EenVoorkeursdokterInstellenStepDefinitions
    {
        private IPatientService _patientService;
        private IDokterRepository _dokterRepository;
        private IPatientRepository _patientRepository;
        private ITijdslotRepository _tijdslotRepository;
        private DokterspraktijkScenarioContext _context;

        private string _huidigePatientVoornaam;
        private string _huidigePatientAchternaam;

        public EenVoorkeursdokterInstellenStepDefinitions(
            IPatientService patientService,
            IDokterRepository dokterRepository,
            IPatientRepository patientRepository,
            ITijdslotRepository tijdslotRepository,
            DokterspraktijkScenarioContext context)
        {
            _patientService = patientService;
            _dokterRepository = dokterRepository;
            _patientRepository = patientRepository;
            _tijdslotRepository = tijdslotRepository;
            _context = context;

            _huidigePatientVoornaam = string.Empty;
            _huidigePatientAchternaam = string.Empty;
        }

        [Given(@"patiënt (.*) (.*) heeft nog geen voorkeursdokter")]
        public void GivenPatientHeeftNogGeenVoorkeursdokter(
            string patientVoornaam,
            string patientAchternaam)
        {
            Patient patient = ZorgDatPatientBestaat(patientVoornaam, patientAchternaam);

            _huidigePatientVoornaam = patient.Voornaam;
            _huidigePatientAchternaam = patient.Achternaam;

            string? voorkeursdokter = _patientService.GeefVoorkeursdokter(
                patient.Voornaam,
                patient.Achternaam);

            Assert.Null(voorkeursdokter);
        }

        [When(@"zij dokter (.*) als voorkeursdokter instelt")]
        public void WhenZijDokterAlsVoorkeursdokterInstelt(string dokterNaam)
        {
            Assert.False(string.IsNullOrWhiteSpace(_huidigePatientVoornaam));
            Assert.False(string.IsNullOrWhiteSpace(_huidigePatientAchternaam));

            Dokter dokter = ZorgDatDokterBestaat(dokterNaam);

            _context.LaatsteResultaat = _patientService.StelVoorkeursdokterIn(
                _huidigePatientVoornaam,
                _huidigePatientAchternaam,
                dokter.Naam);
        }

        [Then(@"wordt dokter (.*) bewaard als haar voorkeursdokter")]
        public void ThenWordtDokterBewaardAlsHaarVoorkeursdokter(string dokterNaam)
        {
            Assert.False(string.IsNullOrWhiteSpace(_huidigePatientVoornaam));
            Assert.False(string.IsNullOrWhiteSpace(_huidigePatientAchternaam));

            Assert.NotNull(_context.LaatsteResultaat);
            Assert.True(_context.LaatsteResultaat.IsGelukt, _context.LaatsteResultaat.Melding);

            string? voorkeursdokter = _patientService.GeefVoorkeursdokter(
                _huidigePatientVoornaam,
                _huidigePatientAchternaam);

            Assert.Equal(dokterNaam, voorkeursdokter);
        }

        [Given(@"patiënt (.*) (.*) heeft dokter (.*) als voorkeursdokter ingesteld")]
        public void GivenPatientHeeftDokterAlsVoorkeursdokterIngesteld(
            string patientVoornaam,
            string patientAchternaam,
            string dokterNaam)
        {
            Patient patient = ZorgDatPatientBestaat(patientVoornaam, patientAchternaam);
            Dokter dokter = ZorgDatDokterBestaat(dokterNaam);

            _huidigePatientVoornaam = patient.Voornaam;
            _huidigePatientAchternaam = patient.Achternaam;

            ResultaatDto resultaat = _patientService.StelVoorkeursdokterIn(
                patient.Voornaam,
                patient.Achternaam,
                dokter.Naam);

            Assert.True(resultaat.IsGelukt, resultaat.Melding);
        }

        [When(@"patiënt (.*) (.*) een nieuwe afspraak wil plannen op (.*)")]
        public void WhenPatientEenNieuweAfspraakWilPlannenOp(
            string patientVoornaam,
            string patientAchternaam,
            string datumTekst)
        {
            Patient patient = ZorgDatPatientBestaat(patientVoornaam, patientAchternaam);

            DateOnly datum = ParseDatum(datumTekst);

            _huidigePatientVoornaam = patient.Voornaam;
            _huidigePatientAchternaam = patient.Achternaam;

            _context.VoorgesteldeVoorkeursdokter = _patientService.GeefVoorkeursdokter(
                patient.Voornaam,
                patient.Achternaam);
        }

        [Then(@"wordt dokter (.*) voorgesteld als voorkeursdokter")]
        public void ThenWordtDokterVoorgesteldAlsVoorkeursdokter(string dokterNaam)
        {
            Assert.Equal(dokterNaam, _context.VoorgesteldeVoorkeursdokter);
        }

        private Patient ZorgDatPatientBestaat(string patientVoornaam, string patientAchternaam)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientVoornaam, patientAchternaam);

            Assert.NotNull(patient);

            return patient;
        }

        private Dokter ZorgDatDokterBestaat(string dokterNaam)
        {
            Dokter? dokter = _dokterRepository.ZoekOpNaam(dokterNaam);

            Assert.NotNull(dokter);

            return dokter;
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