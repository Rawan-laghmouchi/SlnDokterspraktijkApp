using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Infrastructure.Data;

namespace Dokterspraktijk.Infrastructure.Repositories
{
    public class TijdslotRepository : ITijdslotRepository
    {
        private AppDbContext _context;

        public TijdslotRepository(AppDbContext context)
        {
            _context = context;
        }

        public Tijdslot? ZoekOpId(int id)
        {
            return _context.Tijdsloten.FirstOrDefault(tijdslot => tijdslot.Id == id);
        }

        public Tijdslot? ZoekVoorDokterOpDatumEnTijd(int dokterId, DateOnly datum, TimeOnly tijd)
        {
            return _context.Tijdsloten.FirstOrDefault(tijdslot =>
                tijdslot.DokterId == dokterId &&
                tijdslot.Datum == datum &&
                tijdslot.Tijd == tijd);
        }

        public List<Tijdslot> GeefTijdslotenVoorDokterOpDatum(int dokterId, DateOnly datum)
        {
            return _context.Tijdsloten
                .Where(tijdslot =>
                    tijdslot.DokterId == dokterId &&
                    tijdslot.Datum == datum)
                .OrderBy(tijdslot => tijdslot.Tijd)
                .ToList();
        }

        public void VoegToe(Tijdslot tijdslot)
        {
            _context.Tijdsloten.Add(tijdslot);
            _context.SaveChanges();
        }

        public void WerkBij(Tijdslot tijdslot)
        {
            _context.Tijdsloten.Update(tijdslot);
            _context.SaveChanges();
        }
    }
}
