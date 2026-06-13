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
        public string PatientVoornaam { get; set; } = string.Empty;
        public string PatientAchternaam { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public string PatientTelefoonnummer { get; set; } = string.Empty;
        public string PatientRijksregisternummer { get; set; } = string.Empty;
        public string DokterNaam { get; set; } = string.Empty;
        public DateOnly Datum { get; set; }
        public TimeOnly Tijd { get; set; }
        public string Reden { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? FotoBestandsnaam { get; set; }
    }
}
