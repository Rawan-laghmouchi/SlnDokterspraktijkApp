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
    [Scope(Feature = "Foto toevoegen aan afspraakaanvraag")]
    public class FotoToevoegenAanAfspraakaanvraagStepDefinitions
    {
        private readonly IAfspraakService _afspraakService;
        private readonly IPatientRepository _patientRepository;
        private readonly IDokterRepository _dokterRepository;
        private readonly ITijdslotRepository _tijdslotRepository;
        private readonly IAfspraakRepository _afspraakRepository;
        private readonly DokterspraktijkScenarioContext _context;

        public FotoToevoegenAanAfspraakaanvraagStepDefinitions(
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

        [Given(@"patiënt (.*) heeft een afspraakaanvraag voor een huidprobleem")]
        public void GivenPatientHeeftEenAfspraakaanvraagVoorEenHuidprobleem(string patientNaam)
        {
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

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
                "huidprobleem");

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
                Status = "Gepland",
                FotoBestandsnaam = afspraak.FotoBestandsnaam
            };
        }

        [When(@"zij een (.*) toevoegt aan de afspraakaanvraag")]
        public void WhenZijEenBestandToevoegtAanDeAfspraakaanvraag(string bestandstype)
        {
            Assert.NotNull(_context.LaatsteAfspraak);

            string bestandsnaam = MaakBestandsnaam(bestandstype);

            _context.LaatsteResultaat = _afspraakService.ValideerBestandVoorAfspraakaanvraag(
                _context.LaatsteAfspraak.Id,
                bestandsnaam);

            if (_context.LaatsteResultaat.IsGelukt)
            {
                _afspraakService.VoegFotoToeAanAfspraak(
                    _context.LaatsteAfspraak.Id,
                    bestandsnaam);
            }
        }

        [Then(@"wordt het bestand (.*)")]
        public void ThenWordtHetBestandResultaat(string verwachtResultaat)
        {
            Assert.NotNull(_context.LaatsteResultaat);

            if (verwachtResultaat == "geaccepteerd")
            {
                Assert.True(_context.LaatsteResultaat.IsGelukt, _context.LaatsteResultaat.Melding);
            }
            else
            {
                Assert.False(_context.LaatsteResultaat.IsGelukt);
            }
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

        private static string MaakBestandsnaam(string bestandstype)
        {
            if (bestandstype == "JPG-bestand")
            {
                return "huidprobleem.jpg";
            }

            if (bestandstype == "PNG-bestand")
            {
                return "huidprobleem.png";
            }

            if (bestandstype == "PDF-bestand")
            {
                return "huidprobleem.pdf";
            }

            return "huidprobleem.onbekend";
        }
    }
}