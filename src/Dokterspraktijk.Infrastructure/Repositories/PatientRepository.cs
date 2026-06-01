using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
