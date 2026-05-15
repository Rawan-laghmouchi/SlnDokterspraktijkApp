using Dokterspraktijk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Repositories
{
    public interface IDokterRepository
    {
        Dokter? ZoekOpId(int id);
        Dokter? ZoekOpNaam(string naam);
        List<Dokter> GeefAlleDokters();
        void VoegToe(Dokter dokter);
    }
}
