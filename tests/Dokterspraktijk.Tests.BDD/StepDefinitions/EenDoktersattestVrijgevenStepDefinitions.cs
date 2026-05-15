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
    [Scope(Feature = "Een doktersattest vrijgeven")]
    public class EenDoktersattestVrijgevenStepDefinitions
    {
        private readonly IAfspraakService _afspraakService;
        private readonly IDoktersattestService _doktersattestService;
        private readonly IPatientRepository _patientRepository;
        private readonly IDokterRepository _dokterRepository;
        private readonly ITijdslotRepository _tijdslotRepository;
        private readonly IAfspraakRepository _afspraakRepository;
        private readonly IDoktersattestRepository _doktersattestRepository;
        private readonly DokterspraktijkScenarioContext _context;

        public EenDoktersattestVrijgevenStepDefinitions(
            IAfspraakService afspraakService,
            IDoktersattestService doktersattestService,
            IPatientRepository patientRepository,
            IDokterRepository dokterRepository,
            ITijdslotRepository tijdslotRepository,
            IAfspraakRepository afspraakRepository,
            IDoktersattestRepository doktersattestRepository,
            DokterspraktijkScenarioContext context)
        {
            _afspraakService = afspraakService;
            _doktersattestService = doktersattestService;
            _patientRepository = patientRepository;
            _dokterRepository = dokterRepository;
            _tijdslotRepository = tijdslotRepository;
            _afspraakRepository = afspraakRepository;
            _doktersattestRepository = doktersattestRepository;
            _context = context;
        }

        [Given(@"dokter (.*) heeft een afgeronde consultatie met patiënt (.*)")]
        public void GivenDokterHeeftEenAfgerondeConsultatieMetPatient(
            string dokterNaam,
            string patientNaam)
        {
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            Patient patient = ZorgDatPatientBestaat(patientNaam);
            Dokter dokter = ZorgDatDokterBestaat(dokterNaam);

            ZorgDatTijdslotBestaat(
                dokter.Id,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            ResultaatDto maakAfspraakResultaat = _afspraakService.MaakAfspraak(
                patient.Naam,
                dokter.Naam,
                datum,
                tijd,
                "consultatie");

            Assert.True(maakAfspraakResultaat.IsGelukt, maakAfspraakResultaat.Melding);

            ResultaatDto afrondenResultaat = _afspraakService.RondConsultatieAf(
                dokter.Naam,
                patient.Naam,
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
                PatientNaam = patient.Naam,
                DokterNaam = dokter.Naam,
                Datum = datum,
                Tijd = tijd,
                Reden = afspraak.Reden,
                Status = "Afgerond",
                FotoBestandsnaam = afspraak.FotoBestandsnaam
            };
        }

        [Given(@"er is een doktersattest opgesteld voor deze consultatie")]
        public void GivenErIsEenDoktersattestOpgesteldVoorDezeConsultatie()
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            Doktersattest? bestaandDoktersattest =
                _doktersattestRepository.ZoekOpAfspraakId(_context.LaatsteAfspraak.Id);

            if (bestaandDoktersattest == null)
            {
                Doktersattest doktersattest = new Doktersattest(
                    _context.LaatsteAfspraak.Id,
                    _context.LaatsteAfspraak.Id);

                _doktersattestRepository.VoegToe(doktersattest);
            }

            Doktersattest? opgesteldDoktersattest =
                _doktersattestRepository.ZoekOpAfspraakId(_context.LaatsteAfspraak.Id);

            Assert.NotNull(opgesteldDoktersattest);

            _context.LaatsteDoktersattest = new DoktersattestDto
            {
                Id = opgesteldDoktersattest.Id,
                AfspraakId = opgesteldDoktersattest.AfspraakId,
                IsVrijgegeven = opgesteldDoktersattest.IsVrijgegeven,
                IsGedownload = opgesteldDoktersattest.IsGedownload
            };
        }

        [When(@"dokter (.*) het doktersattest vrijgeeft")]
        public void WhenDokterHetDoktersattestVrijgeeft(string dokterNaam)
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            _context.LaatsteResultaat = _doktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                _context.LaatsteAfspraak.PatientNaam,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);

            _context.LaatsteDoktersattest = _doktersattestService.ZoekDoktersattest(
                _context.LaatsteAfspraak.PatientNaam,
                dokterNaam,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);
        }

        [Then(@"wordt het attest beschikbaar voor de patiënt")]
        public void ThenWordtHetAttestBeschikbaarVoorDePatient()
        {
            Assert.NotNull(_context.LaatsteResultaat);
            Assert.True(_context.LaatsteResultaat.IsGelukt, _context.LaatsteResultaat.Melding);

            Assert.NotNull(_context.LaatsteDoktersattest);
            Assert.True(_context.LaatsteDoktersattest.IsVrijgegeven);
        }

        [Given(@"dokter (.*) heeft een geplande consultatie met patiënt (.*)")]
        public void GivenDokterHeeftEenGeplandeConsultatieMetPatient(
            string dokterNaam,
            string patientNaam)
        {
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            Patient patient = ZorgDatPatientBestaat(patientNaam);
            Dokter dokter = ZorgDatDokterBestaat(dokterNaam);

            ZorgDatTijdslotBestaat(
                dokter.Id,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            ResultaatDto maakAfspraakResultaat = _afspraakService.MaakAfspraak(
                patient.Naam,
                dokter.Naam,
                datum,
                tijd,
                "consultatie");

            Assert.True(maakAfspraakResultaat.IsGelukt, maakAfspraakResultaat.Melding);

            Afspraak? afspraak = _afspraakRepository.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                datum,
                tijd);

            Assert.NotNull(afspraak);

            _context.LaatsteAfspraak = new AfspraakDto
            {
                Id = afspraak.Id,
                PatientNaam = patient.Naam,
                DokterNaam = dokter.Naam,
                Datum = datum,
                Tijd = tijd,
                Reden = afspraak.Reden,
                Status = "Gepland",
                FotoBestandsnaam = afspraak.FotoBestandsnaam
            };
        }

        [When(@"dokter (.*) een doktersattest probeert vrij te geven")]
        public void WhenDokterEenDoktersattestProbeertVrijTeGeven(string dokterNaam)
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            _context.LaatsteResultaat = _doktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                _context.LaatsteAfspraak.PatientNaam,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);
        }

        [Then(@"wordt deze actie geweigerd")]
        public void ThenWordtDezeActieGeweigerd()
        {
            Assert.NotNull(_context.LaatsteResultaat);
            Assert.False(_context.LaatsteResultaat.IsGelukt);
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
    }
}