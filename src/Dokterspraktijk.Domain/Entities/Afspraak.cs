using Dokterspraktijk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Domain.Entities
{
    public class Afspraak
    {
        public int Id { get; private set; }
        public int PatientId { get; private set; }
        public int DokterId { get; private set; }
        public int TijdslotId { get; private set; }
        public string Reden { get; private set; }
        public AfspraakStatus Status { get; private set; }
        public string? FotoBestandsnaam { get; private set; }

        public Afspraak(int id, int patientId, int dokterId, int tijdslotId, string reden)
        {
            Id = id;
            PatientId = patientId;
            DokterId = dokterId;
            TijdslotId = tijdslotId;
            Reden = reden;
            Status = AfspraakStatus.Gepland;
            FotoBestandsnaam = null;
        }

        public void VoegFotoToe(string fotoBestandsnaam)
        {
            FotoBestandsnaam = fotoBestandsnaam;
        }

        public bool KanGeannuleerdWorden()
        {
            return Status == AfspraakStatus.Gepland;
        }

        public bool KanAfgerondWorden()
        {
            return Status != AfspraakStatus.Geannuleerd &&
                   Status != AfspraakStatus.Afgerond;
        }

        public void Annuleer()
        {
            Status = AfspraakStatus.Geannuleerd;
        }

        public void RondAf()
        {
            Status = AfspraakStatus.Afgerond;
        }

        public bool IsAfgerond()
        {
            return Status == AfspraakStatus.Afgerond;
        }

        public bool IsGeannuleerd()
        {
            return Status == AfspraakStatus.Geannuleerd;
        }

    }
}
