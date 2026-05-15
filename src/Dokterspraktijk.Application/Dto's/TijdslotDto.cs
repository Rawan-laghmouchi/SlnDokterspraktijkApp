using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Dto_s
{
    public class TijdslotDto
    {
        public int Id { get; set; }
        public string DokterNaam { get; set; }
        public DateOnly Datum { get; set; }
        public TimeOnly Tijd { get; set; }
        public string Status { get; set; }

        public TijdslotDto()
        {
            DokterNaam = string.Empty;
            Status = string.Empty;
        }
    }
}
