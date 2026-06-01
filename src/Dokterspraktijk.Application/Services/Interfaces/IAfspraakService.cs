using Dokterspraktijk.Application.Dto_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Services.Interfaces
{
    public interface IAfspraakService
    {
        ResultaatDto MaakAfspraak(string patientVoornaam, string patientAchternaam, string email, string telefoonnummer, string rijksregisternummer, string dokterNaam, DateOnly datum, TimeOnly tijd, string reden);
        ResultaatDto VoegFotoToeAanAfspraak(int afspraakId, string fotoBestandsnaam);
        List<AfspraakDto> GeefKomendeAfspraken(string patientVoornaam, string patientAchternaam, DateOnly vanafDatum);
        List<AfspraakDto> GeefAlleAfspraken();
        AfspraakDto? ZoekAfspraak(string patientVoornaam, string patientAchternaam, string dokterNaam, DateOnly datum,TimeOnly tijd);

        AfspraakDto? ZoekAfspraakOpId(int id);
        ResultaatDto AnnuleerAfspraak(string patientVoornaam, string patientAchternaam, string dokterNaam, DateOnly datum, TimeOnly tijd);
        ResultaatDto RondConsultatieAf(string dokterNaam, string patientVoornaam, string atientAchternaam, DateOnly datum, TimeOnly tijd);
        ResultaatDto ValideerBestandVoorAfspraakaanvraag(int afspraakId, string bestandsnaam);
    }
}
