using Dokterspraktijk.Application.Dto_s;

namespace Dokterspraktijk.Application.Services.Interfaces
{
    public interface IPatientService
    {
        ResultaatDto StelVoorkeursdokterIn(string patientVoornaam, string patientAchternaam, string dokterNaam);
        string? GeefVoorkeursdokter(string patientVoornaam, string patientAchternaam);
    }
}
