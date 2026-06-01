using Dokterspraktijk.Domain.Enums;

namespace Dokterspraktijk.Domain.Entities
{
    public class Afspraak
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DokterId { get; set; }
        public int TijdslotId { get; set; }
        public string Reden { get; set; }
        public AfspraakStatus Status { get; set; }
        public string? FotoBestandsnaam { get; set; }

        public Afspraak()
        {
            Reden = string.Empty;
            Status = AfspraakStatus.Gepland;
            FotoBestandsnaam = null;
        }

        public Afspraak(int patientId, int dokterId, int tijdslotId, string reden)
        {
            PatientId = patientId;
            DokterId = dokterId;
            TijdslotId = tijdslotId;
            Reden = reden;
            Status = AfspraakStatus.Gepland;
            FotoBestandsnaam = null;
        }
    }
}