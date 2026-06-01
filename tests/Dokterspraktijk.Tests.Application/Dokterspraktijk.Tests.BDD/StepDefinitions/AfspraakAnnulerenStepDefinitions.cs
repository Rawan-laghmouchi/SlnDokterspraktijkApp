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
    [Scope(Feature = "Een afspraak annuleren")]
    public class AfspraakAnnulerenStepDefinitions
    {
        private readonly IAfspraakService _afspraakService;
        private readonly IPatientRepository _patientRepository;
        private readonly IDokterRepository _dokterRepository;
        private readonly ITijdslotRepository _tijdslotRepository;
        private readonly IAfspraakRepository _afspraakRepository;
        private readonly DokterspraktijkScenarioContext _context;

        public AfspraakAnnulerenStepDefinitions(
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

        [Given(@"patiënt (.*) (.*) heeft een afspraak bij dokter (.*) op (.*) om (.*)")]
        public void GivenPatientHeeftEenAfspraakBijDokterOpOm(
            string patientVoornaam,
            string patientAchternaam,
            string dokterNaam,
            string datumTekst,
            string tijdTekst)
        {
            DateOnly datum = ParseDatum(datumTekst);
            TimeOnly tijd = TimeOnly.Parse(tijdTekst);

            Patient patient = ZorgDatPatientBestaat(patientVoornaam, patientAchternaam);
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
                "consultatie");

            Assert.True(resultaat.IsGelukt, resultaat.Melding);

            Afspraak? afspraak = _afspraakRepository.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                datum,
                tijd);

            Assert.NotNull(afspraak);

            _context.LaatsteAfspraak = new AfspraakDto
            {
                Id = afspraak.Id,
                PatientVoornaam = patient.Voornaam,
                PatientAchternaam = patient.Achternaam,
                DokterNaam = dokter.Naam,
                Datum = datum,
                Tijd = tijd,
                Reden = afspraak.Reden,
                Status = "Gepland",
                FotoBestandsnaam = afspraak.FotoBestandsnaam
            };
        }

        [When(@"zij deze afspraak annuleert")]
        public void WhenZijDezeAfspraakAnnuleert()
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            _context.LaatsteResultaat = _afspraakService.AnnuleerAfspraak(
                _context.LaatsteAfspraak.PatientVoornaam,
                _context.LaatsteAfspraak.PatientAchternaam,
                _context.LaatsteAfspraak.DokterNaam,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);
        }

        [Then(@"wordt de afspraak geannuleerd")]
        public void ThenWordtDeAfspraakGeannuleerd()
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            Patient? patient = _patientRepository.ZoekOpNaam(
                _context.LaatsteAfspraak.PatientVoornaam,
                _context.LaatsteAfspraak.PatientAchternaam);

            Dokter? dokter = _dokterRepository.ZoekOpNaam(_context.LaatsteAfspraak.DokterNaam);

            Assert.NotNull(patient);
            Assert.NotNull(dokter);

            Afspraak? afspraak = _afspraakRepository.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);

            Assert.NotNull(afspraak);
            Assert.Equal(AfspraakStatus.Geannuleerd, afspraak.Status);
        }

        [Then(@"wordt het tijdslot opnieuw beschikbaar")]
        public void ThenWordtHetTijdslotOpnieuwBeschikbaar()
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            Dokter? dokter = _dokterRepository.ZoekOpNaam(_context.LaatsteAfspraak.DokterNaam);

            Assert.NotNull(dokter);

            Tijdslot? tijdslot = _tijdslotRepository.ZoekVoorDokterOpDatumEnTijd(
                dokter.Id,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);

            Assert.NotNull(tijdslot);
            Assert.Equal(TijdslotStatus.Beschikbaar, tijdslot.Status);
        }

        [Given(@"patiënt (.*) (.*) heeft een afgeronde afspraak bij de dokter (.*) op (.*)")]
        public void GivenPatientHeeftEenAfgerondeAfspraakBijDeDokterOp(
            string patientVoornaam,
            string patientAchternaam,
            string dokterNaam,
            string datumTekst)
        {
            DateOnly datum = ParseDatum(datumTekst);
            TimeOnly tijd = new TimeOnly(10, 30);

            Patient patient = ZorgDatPatientBestaat(patientVoornaam, patientAchternaam);
            Dokter dokter = ZorgDatDokterBestaat(dokterNaam);

            ZorgDatTijdslotBestaat(
                dokter.Id,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            ResultaatDto maakAfspraakResultaat = _afspraakService.MaakAfspraak(
                patient.Voornaam,
                patient.Achternaam,
                patient.Email,
                patient.Telefoonnummer,
                patient.Rijksregisternummer,
                dokter.Naam,
                datum,
                tijd,
                "consultatie");

            Assert.True(maakAfspraakResultaat.IsGelukt, maakAfspraakResultaat.Melding);

            ResultaatDto afrondenResultaat = _afspraakService.RondConsultatieAf(
                dokter.Naam,
                patient.Voornaam,
                patient.Achternaam,
                datum,
                tijd);

            Assert.True(afrondenResultaat.IsGelukt, afrondenResultaat.Melding);

            Afspraak? afspraak = _afspraakRepository.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                datum,
                tijd);

            Assert.NotNull(afspraak);

            _context.LaatsteAfspraak = new AfspraakDto
            {
                Id = afspraak.Id,
                PatientVoornaam = patient.Voornaam,
                PatientAchternaam = patient.Achternaam,
                DokterNaam = dokter.Naam,
                Datum = datum,
                Tijd = tijd,
                Reden = afspraak.Reden,
                Status = "Afgerond",
                FotoBestandsnaam = afspraak.FotoBestandsnaam
            };
        }

        [When(@"zij deze afspraak probeert te annuleren")]
        public void WhenZijDezeAfspraakProbeertTeAnnuleren()
        {
            WhenZijDezeAfspraakAnnuleert();
        }

        [Then(@"wordt de annulatie geweigerd")]
        public void ThenWordtDeAnnulatieGeweigerd()
        {
            Assert.NotNull(_context.LaatsteResultaat);
            Assert.False(_context.LaatsteResultaat.IsGelukt);
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
    }
}