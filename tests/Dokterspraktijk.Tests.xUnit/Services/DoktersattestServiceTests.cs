using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;
using Dokterspraktijk.Tests.xUnit.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            string patientNaam = "Rawan";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            MaakAfgerondeAfspraak(context, patientNaam, dokterNaam, datum, tijd);

            // Act
            ResultaatDto resultaat = context.DoktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                patientNaam,
                datum,
                tijd);

            DoktersattestDto? doktersattest = context.DoktersattestService.ZoekDoktersattest(
                patientNaam,
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

            string patientNaam = "Rawan";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            MaakGeplandeAfspraak(context, patientNaam, dokterNaam, datum, tijd);

            // Act
            ResultaatDto resultaat = context.DoktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                patientNaam,
                datum,
                tijd);

            DoktersattestDto? doktersattest = context.DoktersattestService.ZoekDoktersattest(
                patientNaam,
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

            string patientNaam = "Rawan";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            MaakAfgerondeAfspraak(context, patientNaam, dokterNaam, datum, tijd);

            ResultaatDto vrijgaveResultaat = context.DoktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                patientNaam,
                datum,
                tijd);

            Assert.True(vrijgaveResultaat.IsGelukt, vrijgaveResultaat.Melding);

            // Act
            ResultaatDto downloadResultaat = context.DoktersattestService.DownloadDoktersattest(
                patientNaam,
                dokterNaam,
                datum,
                tijd);

            DoktersattestDto? doktersattest = context.DoktersattestService.ZoekDoktersattest(
                patientNaam,
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

            string patientNaam = "Rawan";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            Afspraak afspraak = MaakAfgerondeAfspraak(
                context,
                patientNaam,
                dokterNaam,
                datum,
                tijd);

            Doktersattest doktersattest = new Doktersattest(
                afspraak.Id,
                afspraak.Id);

            context.DoktersattestRepository.VoegToe(doktersattest);

            // Act
            ResultaatDto resultaat = context.DoktersattestService.DownloadDoktersattest(
                patientNaam,
                dokterNaam,
                datum,
                tijd);

            DoktersattestDto? opgehaaldDoktersattest = context.DoktersattestService.ZoekDoktersattest(
                patientNaam,
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

            string patientNaam = "Rawan";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            MaakAfgerondeAfspraak(
                context,
                patientNaam,
                dokterNaam,
                datum,
                tijd);

            // Act
            ResultaatDto resultaat = context.DoktersattestService.DownloadDoktersattest(
                patientNaam,
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

            string patientNaam = "Rawan";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            MaakAfgerondeAfspraak(
                context,
                patientNaam,
                dokterNaam,
                datum,
                tijd);

            ResultaatDto vrijgaveResultaat = context.DoktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                patientNaam,
                datum,
                tijd);

            Assert.True(vrijgaveResultaat.IsGelukt, vrijgaveResultaat.Melding);

            // Act
            DoktersattestDto? doktersattest = context.DoktersattestService.ZoekDoktersattest(
                patientNaam,
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

            string patientNaam = "Rawan";
            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);
            TimeOnly tijd = new TimeOnly(10, 30);

            // Act
            ResultaatDto resultaat = context.DoktersattestService.GeefDoktersattestVrij(
                dokterNaam,
                patientNaam,
                datum,
                tijd);

            DoktersattestDto? doktersattest = context.DoktersattestService.ZoekDoktersattest(
                patientNaam,
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
           string patientNaam,
           string dokterNaam,
           DateOnly datum,
           TimeOnly tijd)
        {
            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                tijd,
                TijdslotStatus.Beschikbaar);

            ResultaatDto resultaat = context.AfspraakService.MaakAfspraak(
                patientNaam,
                dokterNaam,
                datum,
                tijd,
                "consultatie");

            Assert.True(resultaat.IsGelukt, resultaat.Melding);

            Afspraak? afspraak = context.ZoekAfspraak(
                patientNaam,
                dokterNaam,
                datum,
                tijd);

            Assert.NotNull(afspraak);

            return afspraak;
        }
        private static Afspraak MaakAfgerondeAfspraak(
           DokterspraktijkServiceTestContext context,
           string patientNaam,
           string dokterNaam,
           DateOnly datum,
           TimeOnly tijd)
        {
            Afspraak afspraak = MaakGeplandeAfspraak(
                context,
                patientNaam,
                dokterNaam,
                datum,
                tijd);

            ResultaatDto resultaat = context.AfspraakService.RondConsultatieAf(
                dokterNaam,
                patientNaam,
                datum,
                tijd);

            Assert.True(resultaat.IsGelukt, resultaat.Melding);

            return afspraak;
        }
    }
}
