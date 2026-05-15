using Dokterspraktijk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Repositories
{
    public interface IPatientRepository
    {
        Patient? ZoekOpId(int id);
        Patient? ZoekOpNaam(string naam);
        void VoegToe(Patient patient);
        void WerkBij(Patient patient);
    }
}
