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
    // US1 – Tijdsloten van een dokter raadplegen
    public class TijdslotServiceTests
    {
        [Fact]
        public void RaadpleegTijdsloten_BestaandeDokterMetTijdsloten_GeeftTijdslotenTerug()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                new TimeOnly(9, 0),
                TijdslotStatus.Beschikbaar);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                new TimeOnly(9, 30),
                TijdslotStatus.NietBeschikbaar);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                new TimeOnly(10, 0),
                TijdslotStatus.Beschikbaar);

            // Act
            List<TijdslotDto> tijdsloten = context.TijdslotService.RaadpleegTijdsloten(
                dokterNaam,
                datum);

            // Assert
            Assert.Equal(3, tijdsloten.Count);

            Assert.Contains(tijdsloten, tijdslot =>
                tijdslot.Tijd == new TimeOnly(9, 0) &&
                tijdslot.Status == "beschikbaar");

            Assert.Contains(tijdsloten, tijdslot =>
                tijdslot.Tijd == new TimeOnly(9, 30) &&
                tijdslot.Status == "niet beschikbaar");

            Assert.Contains(tijdsloten, tijdslot =>
                tijdslot.Tijd == new TimeOnly(10, 0) &&
                tijdslot.Status == "beschikbaar");
        }

        [Fact]
        public void RaadpleegTijdsloten_OnbestaandeDokter_GeeftLegeLijstTerug()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string dokterNaam = "Brancaert";
            DateOnly datum = new DateOnly(2026, 5, 15);

            // Act
            List<TijdslotDto> tijdsloten = context.TijdslotService.RaadpleegTijdsloten(
                dokterNaam,
                datum);

            // Assert
            Assert.Empty(tijdsloten);
        }
        [Fact]
        public void RaadpleegTijdsloten_MeerdereTijdsloten_GeeftTijdslotenGesorteerdOpTijdTerug()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                new TimeOnly(10, 0),
                TijdslotStatus.Beschikbaar);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                new TimeOnly(9, 0),
                TijdslotStatus.Beschikbaar);

            context.VoegTijdslotToe(
                dokterNaam,
                datum,
                new TimeOnly(9, 30),
                TijdslotStatus.NietBeschikbaar);

            // Act
            List<TijdslotDto> tijdsloten = context.TijdslotService.RaadpleegTijdsloten(
                dokterNaam,
                datum);

            // Assert
            Assert.Equal(3, tijdsloten.Count);

            Assert.Equal(new TimeOnly(9, 0), tijdsloten[0].Tijd);
            Assert.Equal("beschikbaar", tijdsloten[0].Status);

            Assert.Equal(new TimeOnly(9, 30), tijdsloten[1].Tijd);
            Assert.Equal("niet beschikbaar", tijdsloten[1].Status);

            Assert.Equal(new TimeOnly(10, 0), tijdsloten[2].Tijd);
            Assert.Equal("beschikbaar", tijdsloten[2].Status);

        }
        [Fact]
        public void RaadpleegTijdsloten_BestaandeDokterZonderTijdsloten_GeeftLegeLijstTerug()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string dokterNaam = "Timmermans";
            DateOnly datum = new DateOnly(2026, 5, 15);

            // Act
            List<TijdslotDto> tijdsloten = context.TijdslotService.RaadpleegTijdsloten(
                dokterNaam,
                datum);

            // Assert
            Assert.Empty(tijdsloten);
        }
    }
}
