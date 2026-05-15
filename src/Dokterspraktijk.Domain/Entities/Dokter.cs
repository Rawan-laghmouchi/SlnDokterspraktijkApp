using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Domain.Entities
{
    public class Dokter
    {
        public int Id { get; private set; }
        public string Naam { get; private set; }
        public string Specialisatie { get; private set; }

        public Dokter(int id, string naam, string specialisatie)
        {
            Id = id;
            Naam = naam;
            Specialisatie = specialisatie;
        }
    }
}
