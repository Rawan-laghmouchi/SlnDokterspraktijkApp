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

            string patientNaam = "Rawan";
            string dokterNaam = "Timmermans";

            // Act
            ResultaatDto resultaat = context.PatientService.StelVoorkeursdokterIn(
                patientNaam,
                dokterNaam);

            string? voorkeursdokter = context.PatientService.GeefVoorkeursdokter(patientNaam);

            // Assert
            Assert.True(resultaat.IsGelukt, resultaat.Melding);
            Assert.Equal(dokterNaam, voorkeursdokter);
        }

        [Fact]
        public void StelVoorkeursdokterIn_OnbestaandePatient_GeeftMisluktResultaat() 
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientNaam = "OnbestaandePatient";
            string dokterNaam = "Timmermans";

            // Act
            ResultaatDto resultaat = context.PatientService.StelVoorkeursdokterIn(
                patientNaam,
                dokterNaam);

            string? voorkeursdokter = context.PatientService.GeefVoorkeursdokter(patientNaam);

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

            string patientNaam = "Rawan";
            string dokterNaam = "Timmermans";

            ResultaatDto resultaat = context.PatientService.StelVoorkeursdokterIn(
                patientNaam,
                dokterNaam);

            Assert.True(resultaat.IsGelukt, resultaat.Melding);

            // Act
            string? voorkeursdokter = context.PatientService.GeefVoorkeursdokter(patientNaam);

            // Assert
            Assert.Equal(dokterNaam, voorkeursdokter);
        }
        [Fact]
        public void GeefVoorkeursdokter_PatientZonderVoorkeursdokter_GeeftNullTerug() 
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientNaam = "Rawan";

            // Act
            string? voorkeursdokter = context.PatientService.GeefVoorkeursdokter(patientNaam);

            // Assert
            Assert.Null(voorkeursdokter);
        }
        [Fact]
        public void GeefVoorkeursdokter_OnbestaandePatient_GeeftNullTerug() 
        {
            // Arrange
            DokterspraktijkServiceTestContext context = new DokterspraktijkServiceTestContext();

            string patientNaam = "OnbestaandePatient";

            // Act
            string? voorkeursdokter = context.PatientService.GeefVoorkeursdokter(patientNaam);

            // Assert
            Assert.Null(voorkeursdokter);
        }

        
    }
}
