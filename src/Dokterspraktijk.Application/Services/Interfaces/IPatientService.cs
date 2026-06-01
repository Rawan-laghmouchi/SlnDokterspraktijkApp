using Dokterspraktijk.Application.Dto_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Services.Interfaces
{
    public interface IPatientService
    {
        ResultaatDto StelVoorkeursdokterIn(string patientVoornaam, string patientAchternaam, string dokterNaam);
        string? GeefVoorkeursdokter(string patientVoornaam, string patientAchternaam);
    }
}
