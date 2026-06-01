using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;
using Dokterspraktijk.Tests.xUnit.Support;

namespace Dokterspraktijk.Tests.xUnit.Services
{
    // US2 – Afspraak maken
    // US3 – Foto toevoegen
    // US5 – Komende afspraken bekijken
    // US6 – Afspraak annuleren
    // US7 – Consultatie afronden
    public class AfspraakServiceTests
    {
        [Fact]
        public void MaakAfspraak_BeschikbaarTijdslot_MaaktAfspraakAan()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            // Act
            ResultaatDto resultaat = context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                dokterNaam,
                datum,
                tijd,
                "consultatie");

            Afspraak? afspraak = context.ZoekAfspraak(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.True(resultaat.IsGelukt, resultaat.Melding);
            Assert.NotNull(afspraak);
            Assert.Equal(AfspraakStatus.Gepland, afspraak.Status);
            Assert.Equal("consultatie", afspraak.Reden);
        }

        [Fact]
        public void MaakAfspraak_BeschikbaarTijdslot_MaaktTijdslotNietBeschikbaar()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            // Act
            context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                dokterNaam,
                datum,
                tijd,
                "consultatie");

            Tijdslot? tijdslot = context.ZoekTijdslot(
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.NotNull(tijdslot);
            Assert.Equal(TijdslotStatus.NietBeschikbaar, tijdslot.Status);
        }

        [Fact]
        public void MaakAfspraak_OnbestaandePatient_MaaktPatientEnAfspraakAan()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Onbestaande";
            string patientAchternaam = "Patient";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(11, 0);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            // Act
            ResultaatDto resultaat = context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "onbestaande.patient@dokterspraktijk.be",
                "0470999999",
                "99.99.99-999.99",
                dokterNaam,
                datum,
                tijd,
                "consultatie");

            Afspraak? afspraak = context.ZoekAfspraak(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.True(resultaat.IsGelukt, resultaat.Melding);
            Assert.NotNull(afspraak);
            Assert.Equal(AfspraakStatus.Gepland, afspraak.Status);
        }

        [Fact]
        public void AnnuleerAfspraak_GeplandeAfspraak_AnnuleertAfspraak()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                dokterNaam,
                datum,
                tijd,
                "consultatie");

            // Act
            ResultaatDto resultaat = context.AfspraakService.AnnuleerAfspraak(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            Afspraak? afspraak = context.ZoekAfspraak(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.True(resultaat.IsGelukt, resultaat.Melding);
            Assert.NotNull(afspraak);
            Assert.Equal(AfspraakStatus.Geannuleerd, afspraak.Status);
        }

        [Fact]
        public void AnnuleerAfspraak_GeplandeAfspraak_MaaktTijdslotOpnieuwBeschikbaar()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                dokterNaam,
                datum,
                tijd,
                "consultatie");

            // Act
            context.AfspraakService.AnnuleerAfspraak(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            Tijdslot? tijdslot = context.ZoekTijdslot(
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.NotNull(tijdslot);
            Assert.Equal(TijdslotStatus.Beschikbaar, tijdslot.Status);
        }

        [Fact]
        public void AnnuleerAfspraak_AfgerondeAfspraak_GeeftMisluktResultaat()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                dokterNaam,
                datum,
                tijd,
                "consultatie");

            context.AfspraakService.RondConsultatieAf(
                dokterNaam,
                patientVoornaam,
                patientAchternaam,
                datum,
                tijd);

            // Act
            ResultaatDto resultaat = context.AfspraakService.AnnuleerAfspraak(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.False(resultaat.IsGelukt);
            Assert.Equal("Een afgeronde afspraak kan niet geannuleerd worden.", resultaat.Melding);
        }

        [Fact]
        public void RondConsultatieAf_GeplandeAfspraak_RondtConsultatieAf()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                dokterNaam,
                datum,
                tijd,
                "consultatie");

            // Act
            ResultaatDto resultaat = context.AfspraakService.RondConsultatieAf(
                dokterNaam,
                patientVoornaam,
                patientAchternaam,
                datum,
                tijd);

            Afspraak? afspraak = context.ZoekAfspraak(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.True(resultaat.IsGelukt, resultaat.Melding);
            Assert.NotNull(afspraak);
            Assert.Equal(AfspraakStatus.Afgerond, afspraak.Status);
        }

        [Fact]
        public void RondConsultatieAf_AfgerondeAfspraak_GeeftMisluktResultaat()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                dokterNaam,
                datum,
                tijd,
                "consultatie");

            context.AfspraakService.RondConsultatieAf(
                dokterNaam,
                patientVoornaam,
                patientAchternaam,
                datum,
                tijd);

            // Act
            ResultaatDto resultaat = context.AfspraakService.RondConsultatieAf(
                dokterNaam,
                patientVoornaam,
                patientAchternaam,
                datum,
                tijd);

            // Assert
            Assert.False(resultaat.IsGelukt);
            Assert.Equal("Deze afspraak kan niet afgerond worden.", resultaat.Melding);
        }

        [Fact]
        public void VoegFotoToeAanAfspraak_BestaandeAfspraak_VoegtFotoToe()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);
            string bestandsnaam = "huidprobleem.jpg";

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                dokterNaam,
                datum,
                tijd,
                "huidprobleem");

            Afspraak? afspraak = context.ZoekAfspraak(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            Assert.NotNull(afspraak);

            // Act
            ResultaatDto resultaat = context.AfspraakService.VoegFotoToeAanAfspraak(
                afspraak.Id,
                bestandsnaam);

            // Assert
            Assert.True(resultaat.IsGelukt, resultaat.Melding);
            Assert.Equal(bestandsnaam, afspraak.FotoBestandsnaam);
        }

        [Fact]
        public void VoegFotoToeAanAfspraak_OnbestaandeAfspraak_GeeftMisluktResultaat()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            // Act
            ResultaatDto resultaat = context.AfspraakService.VoegFotoToeAanAfspraak(
                999,
                "huidprobleem.jpg");

            // Assert
            Assert.False(resultaat.IsGelukt);
            Assert.Equal("De afspraak werd niet gevonden.", resultaat.Melding);
        }

        [Fact]
        public void GeefKomendeAfspraken_PatientMetAfspraken_GeeftKomendeAfsprakenGesorteerdTerug()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";

            context.VoegTijdslotToe(
                "Timmermans",
                new DateOnly(2026, 5, 20),
                new TimeOnly(9, 0),
                TijdslotStatus.Beschikbaar);

            context.VoegTijdslotToe(
                "Brancaert",
                new DateOnly(2026, 5, 15),
                new TimeOnly(10, 30),
                TijdslotStatus.Beschikbaar);

            context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                "Timmermans",
                new DateOnly(2026, 5, 20),
                new TimeOnly(9, 0),
                "huidcontrole");

            context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                "Brancaert",
                new DateOnly(2026, 5, 15),
                new TimeOnly(10, 30),
                "algemene consultatie");

            // Act
            List<AfspraakDto> afspraken = context.AfspraakService.GeefKomendeAfspraken(
                patientVoornaam,
                patientAchternaam,
                new DateOnly(2026, 1, 1));

            // Assert
            Assert.Equal(2, afspraken.Count);
            Assert.Equal(new DateOnly(2026, 5, 15), afspraken[0].Datum);
            Assert.Equal(new DateOnly(2026, 5, 20), afspraken[1].Datum);
        }

        [Fact]
        public void GeefKomendeAfspraken_OnbestaandPatient_GeeftLegeLijstTerug()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            // Act
            List<AfspraakDto> afspraken = context.AfspraakService.GeefKomendeAfspraken(
                "Onbestaande",
                "Patient",
                new DateOnly(2026, 1, 1));

            // Assert
            Assert.Empty(afspraken);
        }

        [Theory]
        [InlineData("huidprobleem.jpg")]
        [InlineData("huidprobleem.jpeg")]
        [InlineData("huidprobleem.png")]
        public void ValideerBestaandeAfspraakAanvraag_GeldigeExtensie_GeeftSucces(string bestandsnaam)
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                dokterNaam,
                datum,
                tijd,
                "huidprobleem");

            Afspraak? afspraak = context.ZoekAfspraak(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            Assert.NotNull(afspraak);

            // Act
            ResultaatDto resultaat = context.AfspraakService.ValideerBestandVoorAfspraakaanvraag(
                afspraak.Id,
                bestandsnaam);

            // Assert
            Assert.True(resultaat.IsGelukt, resultaat.Melding);
        }

        [Theory]
        [InlineData("document.pdf")]
        [InlineData("bestand.docx")]
        [InlineData("foto.gif")]
        public void ValideerBestandVoorAfspraakaanvraag_OngeldigeExtensie_GeeftMisluktResultaat(string bestandsnaam)
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                dokterNaam,
                datum,
                tijd,
                "huidprobleem");

            Afspraak? afspraak = context.ZoekAfspraak(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            Assert.NotNull(afspraak);

            // Act
            ResultaatDto resultaat = context.AfspraakService.ValideerBestandVoorAfspraakaanvraag(
                afspraak.Id,
                bestandsnaam);

            // Assert
            Assert.False(resultaat.IsGelukt);
            Assert.Equal("Het bestandstype wordt niet ondersteund.", resultaat.Melding);
        }

        [Fact]
        public void GeefKomendeAfspraken_MeerderePatienten_GeeftEnkelAfsprakenVanGevraagdePatientTerug()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            DateOnly datumRawan = new DateOnly(2026, 5, 15);
            TimeOnly tijdRawan = new TimeOnly(10, 30);

            DateOnly datumHans = new DateOnly(2026, 5, 20);
            TimeOnly tijdHans = new TimeOnly(9, 0);

            context.VoegTijdslotToe(
                "Timmermans",
                datumRawan,
                tijdRawan,
                TijdslotStatus.Beschikbaar);

            context.VoegTijdslotToe(
                "Brancaert",
                datumHans,
                tijdHans,
                TijdslotStatus.Beschikbaar);

            context.AfspraakService.MaakAfspraak(
                "Rawan",
                "Laghmouchi",
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                "Timmermans",
                datumRawan,
                tijdRawan,
                "algemene consultatie");

            context.AfspraakService.MaakAfspraak(
                "Hans",
                "Vandenbogaerde",
                "hans.vandenbogaerde@gmail.be",
                "0470654321",
                "11.11.11-111.11",
                "Brancaert",
                datumHans,
                tijdHans,
                "huidcontrole");

            // Act
            List<AfspraakDto> afspraken = context.AfspraakService.GeefKomendeAfspraken(
                "Rawan",
                "Laghmouchi",
                new DateOnly(2026, 1, 1));

            // Assert
            Assert.Single(afspraken);
            Assert.Equal("Rawan", afspraken[0].PatientVoornaam);
            Assert.Equal("Laghmouchi", afspraken[0].PatientAchternaam);
            Assert.Equal("Timmermans", afspraken[0].DokterNaam);
            Assert.Equal("algemene consultatie", afspraken[0].Reden);
        }

        [Fact]
        public void MaakAfspraak_NietBeschikbaarTijdslot_GeeftMisluktResultaat()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.NietBeschikbaar);

            // Act
            ResultaatDto resultaat = context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                "rawan.laghmouchi@gmail.be",
                "0470123456",
                "00.00.00-000.00",
                dokterNaam,
                datum,
                tijd,
                "consultatie");

            Afspraak? afspraak = context.ZoekAfspraak(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            Tijdslot? tijdslot = context.ZoekTijdslot(
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.False(resultaat.IsGelukt);
            Assert.Equal("Het gekozen tijdslot is niet beschikbaar.", resultaat.Melding);
            Assert.Null(afspraak);

            Assert.NotNull(tijdslot);
            Assert.Equal(TijdslotStatus.NietBeschikbaar, tijdslot.Status);
        }
    }
}