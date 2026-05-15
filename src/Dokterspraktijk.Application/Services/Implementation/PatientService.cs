using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;

namespace Dokterspraktijk.Application.Services.Implementation
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IDokterRepository _dokterRepository;

        public PatientService(
            IPatientRepository patientRepository,
            IDokterRepository dokterRepository)
        {
            _patientRepository = patientRepository;
            _dokterRepository = dokterRepository;
        }

        public ResultaatDto StelVoorkeursdokterIn(string patientNaam, string dokterNaam)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientNaam);

            if (patient == null)
            {
                return ResultaatDto.Mislukt("De patiënt werd niet gevonden.");
            }

            Dokter? dokter = _dokterRepository.ZoekOpNaam(dokterNaam);

            if (dokter == null)
            {
                return ResultaatDto.Mislukt("De dokter werd niet gevonden.");
            }

            patient.StelVoorkeursdokterIn(dokter.Id);
            _patientRepository.WerkBij(patient);

            return ResultaatDto.Succes("De voorkeursdokter werd ingesteld.");
        }

        public string? GeefVoorkeursdokter(string patientNaam)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientNaam);

            if (patient == null)
            {
                return null;
            }

            if (patient.VoorkeursdokterId == null)
            {
                return null;
            }

            Dokter? dokter = _dokterRepository.ZoekOpId(patient.VoorkeursdokterId.Value);

            if (dokter == null)
            {
                return null;
            }

            return dokter.Naam;
        }
    }
}