using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;
using Dokterspraktijk.Tests.xUnit.Support;

namespace Dokterspraktijk.Tests.xUnit.Services
{
    // US8 – Doktersattest vrijgeven
    // US9 – Doktersattest downloaden
    public class DoktersattestServiceTests
    {
        [Fact]
        public void GeefDoktersattestVrij_AfgerondeConsultatie_GeeftDoktersattestVrij()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Sara";
            string patientAchternaam = "Peeters";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            MaakAfgerondeAfspraak(context, patientVoornaam, patientAchternaam, dokterNaam, datum, tijd);

            // Act
            ResultaatDto resultaat = context.DoktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                patientVoornaam,
                patientAchternaam,
                datum,
                tijd);

            DoktersattestDto? doktersattest = context.DoktersattestService.ZoekDoktersattest(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.True(resultaat.IsGelukt, resultaat.Melding);
            Assert.NotNull(doktersattest);
            Assert.True(doktersattest.IsVrijgegeven);
            Assert.False(doktersattest.IsGedownload);
        }

        [Fact]
        public void GeefDoktersattestVrij_NietAfgerondeConsultatie_GeeftMisluktResultaat()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Sara";
            string patientAchternaam = "Peeters";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            MaakGeplandeAfspraak(context, patientVoornaam, patientAchternaam, dokterNaam, datum, tijd);

            // Act
            ResultaatDto resultaat = context.DoktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                patientVoornaam,
                patientAchternaam,
                datum,
                tijd);

            DoktersattestDto? doktersattest = context.DoktersattestService.ZoekDoktersattest(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.False(resultaat.IsGelukt);
            Assert.Null(doktersattest);
        }

        [Fact]
        public void DownloadDoktersattest_VrijgegevenDoktersattest_MarkeertAlsGedownload()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Sara";
            string patientAchternaam = "Peeters";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            MaakAfgerondeAfspraak(context, patientVoornaam, patientAchternaam, dokterNaam, datum, tijd);

            ResultaatDto vrijgaveResultaat = context.DoktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                patientVoornaam,
                patientAchternaam,
                datum,
                tijd);

            Assert.True(vrijgaveResultaat.IsGelukt, vrijgaveResultaat.Melding);

            // Act
            ResultaatDto downloadResultaat = context.DoktersattestService.DownloadDoktersattest(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            DoktersattestDto? doktersattest = context.DoktersattestService.ZoekDoktersattest(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.True(downloadResultaat.IsGelukt, downloadResultaat.Melding);
            Assert.NotNull(doktersattest);
            Assert.True(doktersattest.IsVrijgegeven);
            Assert.True(doktersattest.IsGedownload);
        }

        [Fact]
        public void DownloadDoktersattest_NietVrijgegevenDoktersattest_GeeftMisluktResultaat()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Sara";
            string patientAchternaam = "Peeters";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            Afspraak afspraak = MaakAfgerondeAfspraak(
                context,
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            Doktersattest doktersattest = new Doktersattest(
                afspraak.Id,
                afspraak.Id);

            context.DoktersattestRepository.VoegToe(doktersattest);

            // Act
            ResultaatDto resultaat = context.DoktersattestService.DownloadDoktersattest(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            DoktersattestDto? opgehaaldDoktersattest = context.DoktersattestService.ZoekDoktersattest(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.False(resultaat.IsGelukt);
            Assert.Equal("Het doktersattest is nog niet vrijgegeven.", resultaat.Melding);

            Assert.NotNull(opgehaaldDoktersattest);
            Assert.False(opgehaaldDoktersattest.IsVrijgegeven);
            Assert.False(opgehaaldDoktersattest.IsGedownload);
        }

        [Fact]
        public void DownloadDoktersattest_OnbestaandDoktersattest_GeeftMisluktResultaat()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Sara";
            string patientAchternaam = "Peeters";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            MaakAfgerondeAfspraak(
                context,
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Act
            ResultaatDto resultaat = context.DoktersattestService.DownloadDoktersattest(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.False(resultaat.IsGelukt);
            Assert.Equal("Er bestaat geen doktersattest voor deze afspraak.", resultaat.Melding);
        }

        [Fact]
        public void ZoekDoktersattest_BestaandDoktersattest_GeeftDoktersattestDtoTerug()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Sara";
            string patientAchternaam = "Peeters";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            MaakAfgerondeAfspraak(
                context,
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            ResultaatDto vrijgaveResultaat = context.DoktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                patientVoornaam,
                patientAchternaam,
                datum,
                tijd);

            Assert.True(vrijgaveResultaat.IsGelukt, vrijgaveResultaat.Melding);

            // Act
            DoktersattestDto? doktersattest = context.DoktersattestService.ZoekDoktersattest(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.NotNull(doktersattest);
            Assert.True(doktersattest.IsVrijgegeven);
            Assert.False(doktersattest.IsGedownload);
        }

        [Fact]
        public void GeefDoktersattestVrij_OnbestaandeAfspraak_GeeftMisluktResultaat()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Sara";
            string patientAchternaam = "Peeters";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            // Act
            ResultaatDto resultaat = context.DoktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                patientVoornaam,
                patientAchternaam,
                datum,
                tijd);

            DoktersattestDto? doktersattest = context.DoktersattestService.ZoekDoktersattest(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            // Assert
            Assert.False(resultaat.IsGelukt);
            Assert.Equal("De afspraak werd niet gevonden.", resultaat.Melding);
            Assert.Null(doktersattest);
        }

        private static Afspraak MaakGeplandeAfspraak(
            DokterspraktijkServiceTestContext context,
            string patientVoornaam,
            string patientAchternaam,
            string dokterNaam,
            DateOnly datum,
            TimeOnly tijd)
        {
            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            string email = GeefEmailVoorPatient(patientVoornaam, patientAchternaam);
            string telefoonnummer = GeefTelefoonnummerVoorPatient(patientVoornaam, patientAchternaam);
            string rijksregisternummer = GeefRijksregisternummerVoorPatient(patientVoornaam, patientAchternaam);

            ResultaatDto resultaat = context.AfspraakService.MaakAfspraak(
                patientVoornaam,
                patientAchternaam,
                email,
                telefoonnummer,
                rijksregisternummer,
                dokterNaam,
                datum,
                tijd,
                "consultatie");

            Assert.True(resultaat.IsGelukt, resultaat.Melding);

            Afspraak? afspraak = context.ZoekAfspraak(
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            Assert.NotNull(afspraak);

            return afspraak;
        }

        private static Afspraak MaakAfgerondeAfspraak(
            DokterspraktijkServiceTestContext context,
            string patientVoornaam,
            string patientAchternaam,
            string dokterNaam,
            DateOnly datum,
            TimeOnly tijd)
        {
            Afspraak afspraak = MaakGeplandeAfspraak(
                context,
                patientVoornaam,
                patientAchternaam,
                dokterNaam,
                datum,
                tijd);

            ResultaatDto resultaat = context.AfspraakService.RondConsultatieAf(
                dokterNaam,
                patientVoornaam,
                patientAchternaam,
                datum,
                tijd);

            Assert.True(resultaat.IsGelukt, resultaat.Melding);

            return afspraak;
        }

        private static string GeefEmailVoorPatient(string patientVoornaam, string patientAchternaam)
        {
            if (patientVoornaam == "Sara" && patientAchternaam == "Peeters")
            {
                return "sara.peeters@gmail.be";
            }

            if (patientVoornaam == "Hans" && patientAchternaam == "Vandenbogaerde")
            {
                return "hans.vandenbogaerde@gmail.be";
            }

            return "test.patient@dokterspraktijk.be";
        }

        private static string GeefTelefoonnummerVoorPatient(string patientVoornaam, string patientAchternaam)
        {
            if (patientVoornaam == "Sara" && patientAchternaam == "Peeters")
            {
                return "0470112233";
            }

            if (patientVoornaam == "Hans" && patientAchternaam == "Vandenbogaerde")
            {
                return "0470654321";
            }

            return "0470123456";
        }

        private static string GeefRijksregisternummerVoorPatient(string patientVoornaam, string patientAchternaam)
        {
            if (patientVoornaam == "Sara" && patientAchternaam == "Peeters")
            {
                return "22.22.22-222.22";
            }

            if (patientVoornaam == "Hans" && patientAchternaam == "Vandenbogaerde")
            {
                return "11.11.11-111.11";
            }

            return "00.00.00-000.00";
        }
    }
}