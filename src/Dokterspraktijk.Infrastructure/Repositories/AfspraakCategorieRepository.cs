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
    public class AfspraakCategorieRepository : IAfspraakCategorieRepository
    {
        private AppDbContext _context;

        public AfspraakCategorieRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<AfspraakCategorie> GeefAlleCategorieen()
        {
            return _context.AfspraakCategorieen
                .OrderBy(categorie => categorie.Naam)
                .ToList();
        }
    }
}
