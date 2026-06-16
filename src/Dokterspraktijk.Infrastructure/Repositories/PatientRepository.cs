using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Infrastructure.Data;

namespace Dokterspraktijk.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }
        public Patient? ZoekOpId(int id)
        {
            return _context.Patienten.FirstOrDefault(patient => patient.Id == id);
        }
        public Patient? ZoekOpNaam(string voornaam, string achternaam)
        {
            return _context.Patienten.FirstOrDefault(patient => 
                patient.Voornaam == voornaam &&
                patient.Achternaam == achternaam);
        }
        public void VoegToe(Patient patient)
        {
            _context.Patienten.Add(patient);
            _context.SaveChanges();
        }
        public void WerkBij(Patient patient)
        {
            _context.Patienten.Update(patient);
            _context.SaveChanges();
        }
        public List<Patient> GeefAllePatienten()
        {
            return _context.Patienten
                .OrderBy(patient => patient.Achternaam)
                .ThenBy(patient => patient.Voornaam)
                .ToList();
        }
        public Patient? ZoekOpEmail(string email)
        {
            return _context.Patienten
                .FirstOrDefault(patient => patient.Email == email);
        }
    }
}
