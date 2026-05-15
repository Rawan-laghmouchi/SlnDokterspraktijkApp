using Dokterspraktijk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Domain.Entities
{
    public class Tijdslot
    {
        public int Id { get; private set; }
        public int DokterId { get; private set; }
        public DateOnly Datum { get; private set; }
        public TimeOnly Tijd { get; private set; }
        public TijdslotStatus Status { get; private set; }

        public Tijdslot(int id, int dokterId, DateOnly datum, TimeOnly tijd, TijdslotStatus status)
        {
            Id = id;
            DokterId = dokterId;
            Datum = datum;
            Tijd = tijd;
            Status = status;
        }
        public void StelIdIn(int id)
        {
            Id = id;
        }
        public bool IsBeschikbaar()
        {
            return Status == TijdslotStatus.Beschikbaar;
        }
        public void MaakNietBeschikbaar()
        {
            Status = TijdslotStatus.NietBeschikbaar;
        }
        public void MaakBeschikbaar()
        {
            Status = TijdslotStatus.Beschikbaar;
        }
    }
}
