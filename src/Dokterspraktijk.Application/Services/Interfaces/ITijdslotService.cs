using Dokterspraktijk.Application.Dto_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Services.Interfaces
{
    public interface ITijdslotService
    {
        List<TijdslotDto> RaadpleegTijdsloten(string dokterNaam, DateOnly datum);
    }
}
