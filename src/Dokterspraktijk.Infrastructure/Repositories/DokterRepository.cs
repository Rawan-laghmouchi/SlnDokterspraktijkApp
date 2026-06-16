using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Infrastructure.Data;

namespace Dokterspraktijk.Infrastructure.Repositories
{
    public class DokterRepository : IDokterRepository
    {

        private AppDbContext _context;

        public DokterRepository(AppDbContext context)
        {
            _context = context;
        }

        public Dokter? ZoekOpId(int id)
        {
            return _context.Dokters.FirstOrDefault(dokter => dokter.Id == id);
        }

        public Dokter? ZoekOpNaam(string naam)
        {
            return _context.Dokters.FirstOrDefault(dokter => dokter.Naam == naam);
        }

        public List<Dokter> GeefAlleDokters()
        {
            return _context.Dokters
                .OrderBy(dokter => dokter.Naam)
                .ToList();
        }

        public void VoegToe(Dokter dokter)
        {
            _context.Dokters.Add(dokter);
            _context.SaveChanges();
        }
        public void WerkBij(Dokter dokter)
        {
            _context.Dokters.Update(dokter);
            _context.SaveChanges();
        }
    }
}
