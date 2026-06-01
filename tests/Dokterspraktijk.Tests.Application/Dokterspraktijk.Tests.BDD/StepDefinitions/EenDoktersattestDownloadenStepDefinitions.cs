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
    public class EenDoktersattestDownloadenStepDefinitions
    {
        private IAfspraakService _afspraakService;
        private IDoktersattestService _doktersattestService;
        private IPatientRepository _patientRepository;
        private IDokterRepository _dokterRepository;
        private ITijdslotRepository _tijdslotRepository;
        private IAfspraakRepository _afspraakRepository;
        private DokterspraktijkScenarioContext _context;

        public EenDoktersattestDownloadenStepDefinitions(
            IAfspraakService afspraakService,
            IDoktersattestService doktersattestService,
            IPatientRepository patientRepository,
            IDokterRepository dokterRepository,
            ITijdslotRepository tijdslotRepository,
            IAfspraakRepository afspraakRepository,
            DokterspraktijkScenarioContext context)
        {
            _afspraakService = afspraakService;
            _doktersattestService = doktersattestService;
            _patientRepository = patientRepository;
            _dokterRepository = dokterRepository;
            _tijdslotRepository = tijdslotRepository;
            _afspraakRepository = afspraakRepository;
            _context = context;
        }

        [Given(@"patiënt (.*) (.*) heeft een afgeronde consultatie")]
        public void GivenPatientHeeftEenAfgerondeConsultatie(
            string patientVoornaam,
            string patientAchternaam)
        {
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
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

        [Given(@"dokter (.*) heeft een doktersattest vrijgegeven")]
        public void GivenDokterHeeftEenDoktersattestVrijgegeven(string dokterNaam)
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            _context.LaatsteResultaat = _doktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                _context.LaatsteAfspraak.PatientVoornaam,
                _context.LaatsteAfspraak.PatientAchternaam,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);

            Assert.True(_context.LaatsteResultaat.IsGelukt, _context.LaatsteResultaat.Melding);

            _context.LaatsteDoktersattest = _doktersattestService.ZoekDoktersattest(
                _context.LaatsteAfspraak.PatientVoornaam,
                _context.LaatsteAfspraak.PatientAchternaam,
                dokterNaam,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);

            Assert.NotNull(_context.LaatsteDoktersattest);
            Assert.True(_context.LaatsteDoktersattest.IsVrijgegeven);
        }

        [Given(@"dokter (.*) heeft het doktersattest nog niet vrijgegeven")]
        public void GivenDokterHeeftHetDoktersattestNogNietVrijgegeven(string dokterNaam)
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            _context.LaatsteDoktersattest = _doktersattestService.ZoekDoktersattest(
                _context.LaatsteAfspraak.PatientVoornaam,
                _context.LaatsteAfspraak.PatientAchternaam,
                dokterNaam,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);

            Assert.Null(_context.LaatsteDoktersattest);
        }

        [When(@"patiënt (.*) (.*) haar doktersattest downloadt")]
        public void WhenPatientHaarDoktersattestDownloadt(
            string patientVoornaam,
            string patientAchternaam)
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            _context.LaatsteResultaat = _doktersattestService.DownloadDoktersattest(
                patientVoornaam,
                patientAchternaam,
                _context.LaatsteAfspraak.DokterNaam,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);

            _context.LaatsteDoktersattest = _doktersattestService.ZoekDoktersattest(
                patientVoornaam,
                patientAchternaam,
                _context.LaatsteAfspraak.DokterNaam,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);
        }

        [When(@"patiënt (.*) (.*) haar doktersattest probeert te downloaden")]
        public void WhenPatientHaarDoktersattestProbeertTeDownloaden(
            string patientVoornaam,
            string patientAchternaam)
        {
            WhenPatientHaarDoktersattestDownloadt(
                patientVoornaam,
                patientAchternaam);
        }

        [Then(@"ontvangt zij het doktersattest van deze consultatie")]
        public void ThenOntvangtZijHetDoktersattestVanDezeConsultatie()
        {
            Assert.NotNull(_context.LaatsteResultaat);
            Assert.True(_context.LaatsteResultaat.IsGelukt, _context.LaatsteResultaat.Melding);

            Assert.NotNull(_context.LaatsteDoktersattest);
            Assert.True(_context.LaatsteDoktersattest.IsVrijgegeven);
            Assert.True(_context.LaatsteDoktersattest.IsGedownload);
        }

        [Then(@"wordt de download geweigerd")]
        public void ThenWordtDeDownloadGeweigerd()
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
    }
}