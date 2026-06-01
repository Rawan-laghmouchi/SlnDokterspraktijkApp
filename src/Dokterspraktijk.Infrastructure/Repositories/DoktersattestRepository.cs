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
    public class DoktersattestRepository : IDoktersattestRepository
    {
        private AppDbContext _context;
        public DoktersattestRepository(AppDbContext context)
        {
            _context = context;
        }
        public Doktersattest? ZoekOpId(int id)
        {
            return _context.Doktersattesten.FirstOrDefault(doktersattest => doktersattest.Id == id);
        }
        public Doktersattest? ZoekOpAfspraakId(int afspraakId)
        {
            return _context.Doktersattesten.FirstOrDefault(doktersattest =>
                doktersattest.AfspraakId == afspraakId);
        }
        public void VoegToe(Doktersattest doktersattest)
        {
            _context.Doktersattesten.Add(doktersattest);
            _context.SaveChanges();
        }
        public void WerkBij(Doktersattest doktersattest)
        {
            _context.Doktersattesten.Update(doktersattest);
            _context.SaveChanges();
        }
    }
}
