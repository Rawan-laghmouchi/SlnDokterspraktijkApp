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
        private readonly IPatientService _patientService;
        private readonly IDokterRepository _dokterRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ITijdslotRepository _tijdslotRepository;
        private readonly DokterspraktijkScenarioContext _context;

        private string _huidigePatientNaam;

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
            _huidigePatientNaam = string.Empty;
        }

        [Given(@"patiënt (.*) heeft nog geen voorkeursdokter")]
        public void GivenPatientHeeftNogGeenVoorkeursdokter(string patientNaam)
        {
            Patient patient = ZorgDatPatientBestaat(patientNaam);

            _huidigePatientNaam = patient.Naam;

            string? voorkeursdokter = _patientService.GeefVoorkeursdokter(patient.Naam);

            Assert.Null(voorkeursdokter);
        }

        [When(@"zij dokter (.*) als voorkeursdokter instelt")]
        public void WhenZijDokterAlsVoorkeursdokterInstelt(string dokterNaam)
        {
            Assert.False(string.IsNullOrWhiteSpace(_huidigePatientNaam));

            Dokter dokter = ZorgDatDokterBestaat(dokterNaam);

            _context.LaatsteResultaat = _patientService.StelVoorkeursdokterIn(
                _huidigePatientNaam,
                dokter.Naam);
        }

        [Then(@"wordt dokter (.*) bewaard als haar voorkeursdokter")]
        public void ThenWordtDokterBewaardAlsHaarVoorkeursdokter(string dokterNaam)
        {
            Assert.False(string.IsNullOrWhiteSpace(_huidigePatientNaam));

            Assert.NotNull(_context.LaatsteResultaat);
            Assert.True(_context.LaatsteResultaat.IsGelukt, _context.LaatsteResultaat.Melding);

            string? voorkeursdokter = _patientService.GeefVoorkeursdokter(_huidigePatientNaam);

            Assert.Equal(dokterNaam, voorkeursdokter);
        }

        [Given(@"patiënt (.*) heeft dokter (.*) als voorkeursdokter ingesteld")]
        public void GivenPatientHeeftDokterAlsVoorkeursdokterIngesteld(
            string patientNaam,
            string dokterNaam)
        {
            Patient patient = ZorgDatPatientBestaat(patientNaam);
            Dokter dokter = ZorgDatDokterBestaat(dokterNaam);

            _huidigePatientNaam = patient.Naam;

            ResultaatDto resultaat = _patientService.StelVoorkeursdokterIn(
                patient.Naam,
                dokter.Naam);

            Assert.True(resultaat.IsGelukt, resultaat.Melding);
        }

        [Given(@"dokter (.*) heeft op (.*) een beschikbaar tijdslot om (.*)")]
        public void GivenDokterHeeftOpEenBeschikbaarTijdslotOm(
            string dokterNaam,
            string datumTekst,
            string tijdTekst)
        {
            DateOnly datum = ParseDatum(datumTekst);
            TimeOnly tijd = ParseTijd(tijdTekst);

            Dokter dokter = ZorgDatDokterBestaat(dokterNaam);

            ZorgDatTijdslotBestaat(
                dokter.Id,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);
        }

        [When(@"patiënt (.*) een nieuwe afspraak wil plannen op (.*)")]
        public void WhenPatientEenNieuweAfspraakWilPlannenOp(
            string patientNaam,
            string datumTekst)
        {
            ZorgDatPatientBestaat(patientNaam);

            DateOnly datum = ParseDatum(datumTekst);

            _huidigePatientNaam = patientNaam;
            _context.VoorgesteldeVoorkeursdokter = _patientService.GeefVoorkeursdokter(patientNaam);
        }

        [Then(@"wordt dokter (.*) voorgesteld als voorkeursdokter")]
        public void ThenWordtDokterVoorgesteldAlsVoorkeursdokter(string dokterNaam)
        {
            Assert.Equal(dokterNaam, _context.VoorgesteldeVoorkeursdokter);
        }

        private Patient ZorgDatPatientBestaat(string patientNaam)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientNaam);

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