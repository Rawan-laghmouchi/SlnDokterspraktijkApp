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
    public class AfspraakMakenBijEenSpecifiekeDokterStepDefinitions
    {
        private readonly IAfspraakService _afspraakService;
        private readonly IPatientRepository _patientRepository;
        private readonly IDokterRepository _dokterRepository;
        private readonly ITijdslotRepository _tijdslotRepository;
        private readonly IAfspraakRepository _afspraakRepository;
        private readonly DokterspraktijkScenarioContext _context;

        public AfspraakMakenBijEenSpecifiekeDokterStepDefinitions(
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

        [Given(@"dokter (.*) heeft op (.*) een niet-beschikbaar tijdslot om (.*)")]
        public void GivenDokterHeeftOpEenNietBeschikbaarTijdslotOm(
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
                TijdslotStatus.NietBeschikbaar);
        }

        [When(@"patiënt (.*) een afspraak bij dokter (.*) op (.*) om (.*) voor een consultatie")]
        public void WhenPatientEenAfspraakBijDokterOpOmVoorEenConsultatie(
            string patientNaam,
            string dokterNaam,
            string datumTekst,
            string tijdTekst)
        {
            DateOnly datum = ParseDatum(datumTekst);
            TimeOnly tijd = ParseTijd(tijdTekst);

            Patient patient = ZorgDatPatientBestaat(patientNaam);
            Dokter dokter = ZorgDatDokterBestaat(dokterNaam);

            _context.LaatsteResultaat = _afspraakService.MaakAfspraak(
                patient.Naam,
                dokter.Naam,
                datum,
                tijd,
                "consultatie");

            Afspraak? afspraak = _afspraakRepository.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                datum,
                tijd);

            if (afspraak != null)
            {
                _context.LaatsteAfspraak = new AfspraakDto
                {
                    Id = afspraak.Id,
                    PatientNaam = patient.Naam,
                    DokterNaam = dokter.Naam,
                    Datum = datum,
                    Tijd = tijd,
                    Reden = afspraak.Reden,
                    Status = VertaalAfspraakStatus(afspraak.Status),
                    FotoBestandsnaam = afspraak.FotoBestandsnaam
                };
            }
        }

        [When(@"patiënt (.*) een afspraak probeert te maken bij dokter (.*) op (.*) om (.*) voor een consultatie")]
        public void WhenPatientEenAfspraakProbeertTeMakenBijDokterOpOmVoorEenConsultatie(
            string patientNaam,
            string dokterNaam,
            string datumTekst,
            string tijdTekst)
        {
            WhenPatientEenAfspraakBijDokterOpOmVoorEenConsultatie(
                patientNaam,
                dokterNaam,
                datumTekst,
                tijdTekst);
        }

        [Then(@"wordt de afspraak geregistreerd")]
        public void ThenWordtDeAfspraakGeregistreerd()
        {
            Assert.NotNull(_context.LaatsteResultaat);
            Assert.True(_context.LaatsteResultaat.IsGelukt, _context.LaatsteResultaat.Melding);

            Assert.NotNull(_context.LaatsteAfspraak);
            Assert.Equal("Gepland", _context.LaatsteAfspraak.Status);
        }

        [Then(@"is het tijdslot van (.*) niet langer beschikbaar")]
        public void ThenIsHetTijdslotVanNietLangerBeschikbaar(string tijdTekst)
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            TimeOnly tijd = ParseTijd(tijdTekst);

            Dokter? dokter = _dokterRepository.ZoekOpNaam(_context.LaatsteAfspraak.DokterNaam);

            Assert.NotNull(dokter);

            Tijdslot? tijdslot = _tijdslotRepository.ZoekVoorDokterOpDatumEnTijd(
                dokter.Id,
                _context.LaatsteAfspraak.Datum,
                tijd);

            Assert.NotNull(tijdslot);
            Assert.Equal(TijdslotStatus.NietBeschikbaar, tijdslot.Status);
        }

        [Then(@"wordt de afspraak geweigerd")]
        public void ThenWordtDeAfspraakGeweigerd()
        {
            Assert.NotNull(_context.LaatsteResultaat);
            Assert.False(_context.LaatsteResultaat.IsGelukt);
        }

        [Then(@"krijgt zij de melding dat het gekozen tijdslot niet beschikbaar is")]
        public void ThenKrijgtZijDeMeldingDatHetGekozenTijdslotNietBeschikbaarIs()
        {
            Assert.NotNull(_context.LaatsteResultaat);
            Assert.Equal(
                "Het gekozen tijdslot is niet beschikbaar.",
                _context.LaatsteResultaat.Melding);
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

        private static string VertaalAfspraakStatus(AfspraakStatus status)
        {
            if (status == AfspraakStatus.Gepland)
            {
                return "Gepland";
            }

            if (status == AfspraakStatus.Geannuleerd)
            {
                return "Geannuleerd";
            }

            return "Afgerond";
        }
    }
}