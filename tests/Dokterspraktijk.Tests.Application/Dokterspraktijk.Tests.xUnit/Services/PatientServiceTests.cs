using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Tests.xUnit.Support;

namespace Dokterspraktijk.Tests.xUnit.Services
{
    // US4 – Voorkeursdokter instellen
    public class PatientServiceTests
    {
        [Fact]
        public void StelVoorkeursdokterIn_BestaandePatientEnDokter_BewaartVoorkeursdokter()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";

            // Act
            ResultaatDto resultaat = context.PatientService.StelVoorkeursdokterIn(
                patientVoornaam,
                patientAchternaam,
                dokterNaam);

            string? voorkeursdokter = context.PatientService.GeefVoorkeursdokter(
                patientVoornaam,
                patientAchternaam);

            // Assert
            Assert.True(resultaat.IsGelukt, resultaat.Melding);
            Assert.Equal(dokterNaam, voorkeursdokter);
        }

        [Fact]
        public void StelVoorkeursdokterIn_OnbestaandePatient_GeeftMisluktResultaat()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Onbestaande";
            string patientAchternaam = "Patient";
            string dokterNaam = "Timmermans";

            // Act
            ResultaatDto resultaat = context.PatientService.StelVoorkeursdokterIn(
                patientVoornaam,
                patientAchternaam,
                dokterNaam);

            string? voorkeursdokter = context.PatientService.GeefVoorkeursdokter(
                patientVoornaam,
                patientAchternaam);

            // Assert
            Assert.False(resultaat.IsGelukt);
            Assert.Equal("De patiënt werd niet gevonden.", resultaat.Melding);
            Assert.Null(voorkeursdokter);
        }

        [Fact]
        public void GeefVoorkeursdokter_PatientMetVoorkeursdokter_GeeftDokterNaamTerug()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";
            string dokterNaam = "Timmermans";

            ResultaatDto resultaat = context.PatientService.StelVoorkeursdokterIn(
                patientVoornaam,
                patientAchternaam,
                dokterNaam);

            Assert.True(resultaat.IsGelukt, resultaat.Melding);

            // Act
            string? voorkeursdokter = context.PatientService.GeefVoorkeursdokter(
                patientVoornaam,
                patientAchternaam);

            // Assert
            Assert.Equal(dokterNaam, voorkeursdokter);
        }

        [Fact]
        public void GeefVoorkeursdokter_PatientZonderVoorkeursdokter_GeeftNullTerug()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Rawan";
            string patientAchternaam = "Laghmouchi";

            // Act
            string? voorkeursdokter = context.PatientService.GeefVoorkeursdokter(
                patientVoornaam,
                patientAchternaam);

            // Assert
            Assert.Null(voorkeursdokter);
        }

        [Fact]
        public void GeefVoorkeursdokter_OnbestaandePatient_GeeftNullTerug()
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientVoornaam = "Onbestaande";
            string patientAchternaam = "Patient";

            // Act
            string? voorkeursdokter = context.PatientService.GeefVoorkeursdokter(
                patientVoornaam,
                patientAchternaam);

            // Assert
            Assert.Null(voorkeursdokter);
        }
    }
}