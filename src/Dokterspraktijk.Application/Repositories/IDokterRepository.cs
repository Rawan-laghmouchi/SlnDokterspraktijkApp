using Dokterspraktijk.Domain.Entities;

namespace Dokterspraktijk.Application.Repositories
{
    public interface IDokterRepository
    {
        Dokter? ZoekOpId(int id);
        Dokter? ZoekOpNaam(string naam);
        List<Dokter> GeefAlleDokters();
        void VoegToe(Dokter dokter);
        void WerkBij(Dokter dokter);
    }
}
