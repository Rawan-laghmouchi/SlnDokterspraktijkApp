using Dokterspraktijk.Domain.Entities;

namespace Dokterspraktijk.Application.Repositories
{
    public interface IPatientRepository
    {
        Patient? ZoekOpId(int id);
        Patient? ZoekOpNaam(string voornaam, string achternaam);
        Patient? ZoekOpEmail(string email);
        List<Patient> GeefAllePatienten();
        void VoegToe(Patient patient);
        void WerkBij(Patient patient);
    }
}
