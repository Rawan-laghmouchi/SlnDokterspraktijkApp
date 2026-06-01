using Dokterspraktijk.Application.Dto_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Services.Interfaces
{
    public interface IDoktersattestService
    {
        ResultaatDto GeefDoktersattestVrij(string dokterNaam, string patientVoornaam, string patientAchternaam, DateOnly datum, TimeOnly tijd);
        ResultaatDto DownloadDoktersattest(string patientVoornaam, string patientAchternaam, string dokterNaam, DateOnly datum, TimeOnly tijd);
        DoktersattestDto? ZoekDoktersattest(string patientVoornaam, string patientAchternaam, string dokterNaam, DateOnly datum, TimeOnly tijd);
    }
}
