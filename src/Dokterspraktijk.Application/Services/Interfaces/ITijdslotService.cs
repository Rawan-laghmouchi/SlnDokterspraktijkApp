using Dokterspraktijk.Application.Dto_s;

namespace Dokterspraktijk.Application.Services.Interfaces
{
    public interface ITijdslotService
    {
        List<TijdslotDto> RaadpleegTijdsloten(string dokterNaam, DateOnly datum);
    }
}
