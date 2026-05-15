using Dokterspraktijk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Repositories
{
    public interface IAfspraakRepository
    {
        Afspraak? ZoekOpId(int id);
        Afspraak? ZoekOpPatientDokterDatumEnTijd(int patientId, int dokterId, DateOnly datum, TimeOnly tijd);
        List<Afspraak> GeefAfsprakenVoorPatient(int patientId);
        void VoegToe(Afspraak afspraak);
        void WerkBij(Afspraak afspraak);
    }
}
