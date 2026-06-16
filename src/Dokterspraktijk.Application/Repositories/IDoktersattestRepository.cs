using Dokterspraktijk.Domain.Entities;

namespace Dokterspraktijk.Application.Repositories
{
    public interface IDoktersattestRepository
    {
        Doktersattest? ZoekOpId(int id);
        Doktersattest? ZoekOpAfspraakId(int afspraakId);
        void VoegToe(Doktersattest doktersattest);
        void WerkBij(Doktersattest doktersattest);
    }
}
