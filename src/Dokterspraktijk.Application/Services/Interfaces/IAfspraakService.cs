using Dokterspraktijk.Application.Dto_s;

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
        ResultaatDto AnnuleerAfspraakOpId(int afspraakId);
        ResultaatDto RondConsultatieAf(string dokterNaam, string patientVoornaam, string patientAchternaam, DateOnly datum, TimeOnly tijd);
        ResultaatDto ValideerBestandVoorAfspraakaanvraag(int afspraakId, string bestandsnaam);
    }
}
