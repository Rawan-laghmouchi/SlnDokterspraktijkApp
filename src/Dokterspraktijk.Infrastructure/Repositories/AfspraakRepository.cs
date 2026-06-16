using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Infrastructure.Data;

namespace Dokterspraktijk.Infrastructure.Repositories
{
    public class AfspraakRepository : IAfspraakRepository
    {
        private AppDbContext _context;

        public AfspraakRepository(AppDbContext context)
        {
            _context = context;
        }

        public Afspraak? ZoekOpId(int id)
        {
            return _context.Afspraken
                .FirstOrDefault(afspraak => afspraak.Id == id);
        }

        public List<Afspraak> GeefAfsprakenVoorPatient(int patientId)
        {
            return _context.Afspraken
                .Where(afspraak => afspraak.PatientId == patientId)
                .ToList();
        }

        public List<Afspraak> GeefAlleAfspraken()
        {
            return _context.Afspraken
                .ToList();
        }

        public Afspraak? ZoekOpPatientDokterDatumEnTijd(
            int patientId,
            int dokterId,
            DateOnly datum,
            TimeOnly tijd)
        {
            return _context.Afspraken
                .Join(
                    _context.Tijdsloten,
                    afspraak => afspraak.TijdslotId,
                    tijdslot => tijdslot.Id,
                    (afspraak, tijdslot) => new
                    {
                        Afspraak = afspraak,
                        Tijdslot = tijdslot
                    })
                .Where(combinatie =>
                    combinatie.Afspraak.PatientId == patientId &&
                    combinatie.Afspraak.DokterId == dokterId &&
                    combinatie.Tijdslot.Datum == datum &&
                    combinatie.Tijdslot.Tijd == tijd)
                .Select(combinatie => combinatie.Afspraak)
                .FirstOrDefault();
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