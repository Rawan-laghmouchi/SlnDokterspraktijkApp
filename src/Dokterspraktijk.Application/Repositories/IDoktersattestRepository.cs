using Dokterspraktijk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
