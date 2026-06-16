using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;

namespace Dokterspraktijk.Application.Services.Implementation
{
    public class PatientService : IPatientService
    {
        private IUnitOfWork _unitOfWork;
        public PatientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ResultaatDto StelVoorkeursdokterIn(string patientVoornaam, string patientAchternaam, string dokterNaam)
        {
            Patient? patient = _unitOfWork.Patienten.ZoekOpNaam(patientVoornaam, patientAchternaam);

            if (patient == null)
            {
                return ResultaatDto.Mislukt("De patiënt werd niet gevonden.");
            }

            Dokter? dokter = _unitOfWork.Dokters.ZoekOpNaam(dokterNaam);

            if (dokter == null)
            {
                return ResultaatDto.Mislukt("De dokter werd niet gevonden.");
            }

            patient.VoorkeursdokterId = dokter.Id;
            _unitOfWork.Patienten.WerkBij(patient);

            return ResultaatDto.Succes("De voorkeursdokter werd ingesteld.");
        }

        public string? GeefVoorkeursdokter(string patientVoornaam, string patientAchternaam)
        {
            Patient? patient = _unitOfWork.Patienten.ZoekOpNaam(patientVoornaam, patientAchternaam);

            if (patient == null)
            {
                return null;
            }

            if (patient.VoorkeursdokterId == null)
            {
                return null;
            }

            Dokter? dokter = _unitOfWork.Dokters.ZoekOpId(patient.VoorkeursdokterId.Value);

            if (dokter == null)
            {
                return null;
            }

            return dokter.Naam;
        }
    }
}