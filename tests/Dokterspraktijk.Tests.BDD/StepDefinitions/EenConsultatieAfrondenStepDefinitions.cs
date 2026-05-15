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
    [Scope(Feature = "Een Consultatie afronden")]
    public class EenConsultatieAfrondenStepDefinitions
    {
        private readonly IAfspraakService _afspraakService;
        private readonly IPatientRepository _patientRepository;
        private readonly IDokterRepository _dokterRepository;
        private readonly ITijdslotRepository _tijdslotRepository;
        private readonly IAfspraakRepository _afspraakRepository;
        private readonly DokterspraktijkScenarioContext _context;

        public EenConsultatieAfrondenStepDefinitions(
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

        [Given(@"dokter (.*) heeft een geplande afspraak met patiënt (.*) op (.*) om (.*)")]
        public void GivenDokterHeeftEenGeplandeAfspraakMetPatientOpOm(
            string dokterNaam,
            string patientNaam,
            string datumTekst,
            string tijdTekst)
        {
            DateOnly datum = ParseDatum(datumTekst);
            TimeOnly tijd = ParseTijd(tijdTekst);

            Patient patient = ZorgDatPatientBestaat(patientNaam);
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
                PatientNaam = patient.Naam,
                DokterNaam = dokter.Naam,
                Datum = datum,
                Tijd = tijd,
                Reden = afspraak.Reden,
                Status = VertaalAfspraakStatus(afspraak.Status),
                FotoBestandsnaam = afspraak.FotoBestandsnaam
            };
        }

        [When(@"dokter (.*) de consultatie afrondt")]
        public void WhenDokterDeConsultatieAfrondt(string dokterNaam)
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            _context.LaatsteResultaat = _afspraakService.RondConsultatieAf(
                dokterNaam,
                _context.LaatsteAfspraak.PatientNaam,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);
        }

        [Then(@"krijgt de afspraak de status ""(.*)""")]
        public void ThenKrijgtDeAfspraakDeStatus(string verwachteStatus)
        {
            Assert.NotNull(_context.LaatsteAfspraak);
            Assert.NotNull(_context.LaatsteResultaat);
            Assert.True(_context.LaatsteResultaat.IsGelukt, _context.LaatsteResultaat.Melding);

            Patient? patient = _patientRepository.ZoekOpNaam(_context.LaatsteAfspraak.PatientNaam);
            Dokter? dokter = _dokterRepository.ZoekOpNaam(_context.LaatsteAfspraak.DokterNaam);

            Assert.NotNull(patient);
            Assert.NotNull(dokter);

            Afspraak? afspraak = _afspraakRepository.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                _context.LaatsteAfspraak.Datum,
                _context.LaatsteAfspraak.Tijd);

            Assert.NotNull(afspraak);
            Assert.Equal(verwachteStatus, VertaalAfspraakStatus(afspraak.Status));
        }

        [Given(@"dokter (.*) heeft een afspraak met status ""Afgerond""")]
        public void GivenDokterHeeftEenAfspraakMetStatusAfgerond(string dokterNaam)
        {
            string patientNaam = "Rawan";
            DateOnly datum = new DateOnly(2026, 5, 10);
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
                Status = VertaalAfspraakStatus(afspraak.Status),
                FotoBestandsnaam = afspraak.FotoBestandsnaam
            };
        }

        [When(@"dokter (.*) de consultatie opnieuw probeert af te ronden")]
        public void WhenDokterDeConsultatieOpnieuwProbeertAfTeRonden(string dokterNaam)
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            _context.LaatsteResultaat = _afspraakService.RondConsultatieAf(
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