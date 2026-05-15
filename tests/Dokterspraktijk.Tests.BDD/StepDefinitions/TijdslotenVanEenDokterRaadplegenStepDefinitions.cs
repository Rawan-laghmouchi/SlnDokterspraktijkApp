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
    [Scope(Feature = "Tijdsloten van een dokter raadplegen")]
    public class TijdslotenVanEenDokterRaadplegenStepDefinitions
    {
        private readonly ITijdslotService _tijdslotService;
        private readonly IDokterRepository _dokterRepository;
        private readonly ITijdslotRepository _tijdslotRepository;
        private readonly DokterspraktijkScenarioContext _context;

        public TijdslotenVanEenDokterRaadplegenStepDefinitions(
            ITijdslotService tijdslotService,
            IDokterRepository dokterRepository,
            ITijdslotRepository tijdslotRepository,
            DokterspraktijkScenarioContext context)
        {
            _tijdslotService = tijdslotService;
            _dokterRepository = dokterRepository;
            _tijdslotRepository = tijdslotRepository;
            _context = context;
        }

        [Given(@"dokter (.*) heeft op (.*) de volgende tijdsloten:")]
        public void GivenDokterHeeftOpDeVolgendeTijdsloten(
            string dokterNaam,
            string datumTekst,
            Table table)
        {
            DateOnly datum = ParseDatum(datumTekst);
            Dokter dokter = ZorgDatDokterBestaat(dokterNaam);

            foreach (var rij in table.Rows)
            {
                TimeOnly tijd = ParseTijd(rij["Tijd"]);
                TijdslotStatus status = ParseTijdslotStatus(rij["Status"]);

                ZorgDatTijdslotBestaat(
                    dokter.Id,
                    datum,
                    tijd,
                    status);
            }
        }

        [When(@"patiënt (.*) de tijdsloten van dokter (.*) op (.*) raadpleegt")]
        public void WhenPatientDeTijdslotenVanDokterOpRaadpleegt(
            string patientNaam,
            string dokterNaam,
            string datumTekst)
        {
            DateOnly datum = ParseDatum(datumTekst);

            _context.GeraadpleegdeTijdsloten = _tijdslotService.RaadpleegTijdsloten(
                dokterNaam,
                datum);
        }

        [Then(@"ziet zij de volgende tijdsloten:")]
        public void ThenZietZijDeVolgendeTijdsloten(Table table)
        {
            Assert.Equal(table.RowCount, _context.GeraadpleegdeTijdsloten.Count);

            foreach (var rij in table.Rows)
            {
                TimeOnly verwachteTijd = ParseTijd(rij["Tijd"]);
                string verwachteStatus = NormaliseerStatus(rij["Status"]);

                TijdslotDto? gevondenTijdslot = _context.GeraadpleegdeTijdsloten.FirstOrDefault(tijdslot =>
                    tijdslot.Tijd == verwachteTijd &&
                    NormaliseerStatus(tijdslot.Status) == verwachteStatus);

                Assert.NotNull(gevondenTijdslot);
            }
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

        private static TijdslotStatus ParseTijdslotStatus(string status)
        {
            string genormaliseerdeStatus = NormaliseerStatus(status);

            if (genormaliseerdeStatus == "beschikbaar")
            {
                return TijdslotStatus.Beschikbaar;
            }

            if (genormaliseerdeStatus == "niet beschikbaar")
            {
                return TijdslotStatus.NietBeschikbaar;
            }

            throw new ArgumentException($"Onbekende tijdslotstatus: {status}");
        }

        private static string NormaliseerStatus(string status)
        {
            return status.Trim().ToLowerInvariant();
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
                tijdTekst.Trim(),
                "HH:mm",
                new CultureInfo("nl-BE"),
                DateTimeStyles.None);
        }
    }
}