using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Domain.Entities
{
    public class Patient
    {
        public int Id { get; private set; }
        public string Naam { get; private set; }
        public int? VoorkeursdokterId { get; private set; }

        public Patient(int id, string naam)
        {
            Id = id;
            Naam = naam;
            VoorkeursdokterId = null;
        }
        public void StelVoorkeursdokterIn(int dokterId)
        {
            VoorkeursdokterId = dokterId;
        }
    }
}
