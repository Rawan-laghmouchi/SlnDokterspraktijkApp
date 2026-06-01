using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dokterspraktijk.Tests.BDD.StepDefinitions
{
    [Binding]
    public class GemeenschappelijkeTijdslotStepDefinitions
    {
        private readonly IDokterRepository _dokterRepository;
        private readonly ITijdslotRepository _tijdslotRepository;

        public GemeenschappelijkeTijdslotStepDefinitions(
            IDokterRepository dokterRepository,
            ITijdslotRepository tijdslotRepository)
        {
            _dokterRepository = dokterRepository;
            _tijdslotRepository = tijdslotRepository;
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
