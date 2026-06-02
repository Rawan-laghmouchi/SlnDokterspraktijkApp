using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Services.Implementation
{
    public class TijdslotService : ITijdslotService
    {
        private IDokterRepository _dokterRepository;
        private ITijdslotRepository _tijdslotRepository;

        public TijdslotService(IDokterRepository dokterRepository, ITijdslotRepository tijdslotRepository) 
        {
            _dokterRepository = dokterRepository;
            _tijdslotRepository = tijdslotRepository;
        }

        public List<TijdslotDto> RaadpleegTijdsloten(string dokterNaam, DateOnly datum)
        {
            Dokter? dokter = _dokterRepository.ZoekOpNaam(dokterNaam);
            if (dokter == null)
            {
                return new List<TijdslotDto>();
            }
            List<Tijdslot> tijdsloten = _tijdslotRepository.GeefTijdslotenVoorDokterOpDatum(dokter.Id, datum);
            List<TijdslotDto> resultaat = new List<TijdslotDto>();

            foreach (Tijdslot tijdslot in tijdsloten)
            {
                resultaat.Add(new TijdslotDto
                {
                    Id = tijdslot.Id,
                    DokterNaam = dokter.Naam,
                    Datum = tijdslot.Datum,
                    Tijd = tijdslot.Tijd,
                    Status = VertaalTijdslotStatus(tijdslot.Status) // nog bekijken
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
