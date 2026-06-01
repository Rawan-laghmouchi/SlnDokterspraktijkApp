using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Infrastructure.Repositories
{
    public class AfspraakRepository : IAfspraakRepository
    {
        private AppDbContext _context;
        private ITijdslotRepository _tijdslotRepository;

        public AfspraakRepository(AppDbContext context, ITijdslotRepository tijdslotRepository)
        {
            _context = context;
            _tijdslotRepository = tijdslotRepository;
        }
        public Afspraak? ZoekOpId(int id)
        {
            return _context.Afspraken.FirstOrDefault(afspraak => afspraak.Id == id);
        }
        public List<Afspraak> GeefAfsprakenVoorPatient(int patientId)
        {
            return _context.Afspraken
                .Where(afspraak => afspraak.PatientId == patientId)
                .ToList();
        }
        public List<Afspraak> GeefAlleAfspraken()
        {
            return _context.Afspraken.ToList();
        }
        public Afspraak? ZoekOpPatientDokterDatumEnTijd(int patientId, int dokterId, DateOnly datum, TimeOnly tijd)
        {
            List<Afspraak> afspraken = _context.Afspraken
                .Where(afspraak =>
                    afspraak.PatientId == patientId &&
                    afspraak.DokterId == dokterId)
                .ToList();

            foreach (Afspraak afspraak in afspraken)
            {
                Tijdslot? tijdslot = _tijdslotRepository.ZoekOpId(afspraak.TijdslotId);

                if (tijdslot != null &&
                    tijdslot.Datum == datum &&
                    tijdslot.Tijd == tijd)
                {
                    return afspraak;
                }
            }

            return null;
        }
        public void VoegToe(Afspraak afspraak)
        {
            _context.Afspraken.Add(afspraak);
            _context.SaveChanges();
        }
        public void WerkBij(Afspraak afspraak)
        {
            _context.Afspraken.Update(afspraak);
            _context.SaveChanges();
        }
    }
}
