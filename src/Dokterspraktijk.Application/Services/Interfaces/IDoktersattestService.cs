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
        ResultaatDto GeefDoktersattestVrij(string dokterNaam, string patientNaam, DateOnly datum, TimeOnly tijd);
        ResultaatDto DownloadDoktersattest(string patientNaam, string dokterNaam, DateOnly datum, TimeOnly tijd);
        DoktersattestDto? ZoekDoktersattest(string patientNaam, string dokterNaam, DateOnly datum, TimeOnly tijd);
    }
}
