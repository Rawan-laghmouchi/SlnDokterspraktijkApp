using Dokterspraktijk.Domain.Enums;

namespace Dokterspraktijk.Domain.Entities
{
    public class Tijdslot
    {
        public int Id { get; set; }
        public int DokterId { get; set; }
        public DateOnly Datum { get; set; }
        public TimeOnly Tijd { get; set; }
        public TijdslotStatus Status { get; set; }

        public Tijdslot()
        {
            Status = TijdslotStatus.Beschikbaar;
        }

        public Tijdslot(int id, int dokterId, DateOnly datum, TimeOnly tijd, TijdslotStatus status)
        {
            Id = id;
            DokterId = dokterId;
            Datum = datum;
            Tijd = tijd;
            Status = status;
        }

        public Tijdslot(int dokterId, DateOnly datum, TimeOnly tijd, TijdslotStatus status)
        {
            DokterId = dokterId;
            Datum = datum;
            Tijd = tijd;
            Status = status;
        }
    }
}