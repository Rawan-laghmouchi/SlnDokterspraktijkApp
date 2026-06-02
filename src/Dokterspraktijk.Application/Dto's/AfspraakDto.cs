using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Dto_s
{
    public class AfspraakDto
    {
        public int Id { get; set; }
        public string PatientVoornaam { get; set; }
        public string PatientAchternaam { get; set; }
        public string DokterNaam { get; set; }
        public DateOnly Datum { get; set; }
        public TimeOnly Tijd { get; set; }
        public string Reden { get; set; }
        public string Status { get; set; }
        public string? FotoBestandsnaam { get; set; }

        public AfspraakDto()
        {
            PatientVoornaam = string.Empty;
            PatientAchternaam = string.Empty;
            DokterNaam = string.Empty;
            Reden = string.Empty;
            Status = string.Empty;
        }
    }
}
