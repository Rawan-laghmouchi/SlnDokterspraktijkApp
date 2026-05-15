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
        ResultaatDto MaakAfspraak(string patientNaam, string dokterNaam, DateOnly datum, TimeOnly tijd, string reden);
        ResultaatDto VoegFotoToeAanAfspraak(int afspraakId, string fotoBestandsnaam);
        List<AfspraakDto> GeefKomendeAfspraken(string patientNaam, DateOnly vanafDatum);
        ResultaatDto AnnuleerAfspraak(string patientNaam, string dokterNaam, DateOnly datum, TimeOnly tijd);
        ResultaatDto RondConsultatieAf(string dokterNaam, string patientNaam, DateOnly datum, TimeOnly tijd);
        ResultaatDto ValideerBestandVoorAfspraakaanvraag(int afspraakId, string bestandsnaam);
    }
}
