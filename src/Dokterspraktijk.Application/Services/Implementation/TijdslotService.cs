using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;


namespace Dokterspraktijk.Application.Services.Implementation
{
    public class TijdslotService : ITijdslotService
    {
        private IUnitOfWork _unitOfWork;
        public TijdslotService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<TijdslotDto> RaadpleegTijdsloten(string dokterNaam, DateOnly datum)
        {
            Dokter? dokter = _unitOfWork.Dokters.ZoekOpNaam(dokterNaam);
            if (dokter == null)
            {
                return new List<TijdslotDto>();
            }
            List<Tijdslot> tijdsloten = _unitOfWork.Tijdsloten.GeefTijdslotenVoorDokterOpDatum(dokter.Id, datum);
            List<TijdslotDto> resultaat = new List<TijdslotDto>();

            foreach (Tijdslot tijdslot in tijdsloten)
            {
                resultaat.Add(new TijdslotDto
                {
                    Id = tijdslot.Id,
                    DokterNaam = dokter.Naam,
                    Datum = tijdslot.Datum,
                    Tijd = tijdslot.Tijd,
                    Status = VertaalTijdslotStatus(tijdslot.Status)
                });
            }
            return resultaat;
        }

        private static string VertaalTijdslotStatus(TijdslotStatus status)
        {
            if (status == TijdslotStatus.Beschikbaar)
            {
                return "beschikbaar";
            }
            return "niet beschikbaar";
        }
    }
}
