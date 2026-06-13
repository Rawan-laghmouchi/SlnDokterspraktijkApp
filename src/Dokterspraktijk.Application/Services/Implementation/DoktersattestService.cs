using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;

namespace Dokterspraktijk.Application.Services.Implementation
{
    public class DoktersattestService : IDoktersattestService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IDokterRepository _dokterRepository;
        private readonly IAfspraakRepository _afspraakRepository;
        private readonly IDoktersattestRepository _doktersattestRepository;

        public DoktersattestService(
            IPatientRepository patientRepository,
            IDokterRepository dokterRepository,
            IAfspraakRepository afspraakRepository,
            IDoktersattestRepository doktersattestRepository)
        {
            _patientRepository = patientRepository;
            _dokterRepository = dokterRepository;
            _afspraakRepository = afspraakRepository;
            _doktersattestRepository = doktersattestRepository;
        }

        public ResultaatDto GeefDoktersattestVrij(
            string dokterNaam,
            string patientVoornaam,
            string patientAchternaam,
            DateOnly datum,
            TimeOnly tijd)
        {
            Afspraak? afspraak = ZoekAfspraak(patientVoornaam, patientAchternaam, dokterNaam, datum, tijd);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }

            return GeefDoktersattestVrijVoorAfspraak(afspraak.Id);
        }

        public ResultaatDto GeefDoktersattestVrijVoorAfspraak(int afspraakId)
        {
            Afspraak? afspraak = _afspraakRepository.ZoekOpId(afspraakId);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }

            if (afspraak.Status != AfspraakStatus.Afgerond)
            {
                return ResultaatDto.Mislukt("Een doktersattest kan pas na een afgeronde consultatie worden vrijgegeven.");
            }

            Doktersattest? doktersattest = _doktersattestRepository.ZoekOpAfspraakId(afspraak.Id);

            if (doktersattest == null)
            {
                doktersattest = new Doktersattest
                {
                    AfspraakId = afspraak.Id,
                    IsVrijgegeven = true,
                    IsGedownload = false
                };

                _doktersattestRepository.VoegToe(doktersattest);
            }
            else
            {
                doktersattest.IsVrijgegeven = true;
                _doktersattestRepository.WerkBij(doktersattest);
            }

            return ResultaatDto.Succes("Het doktersattest werd vrijgegeven.");
        }

        public ResultaatDto DownloadDoktersattest(
            string patientVoornaam,
            string patientAchternaam,
            string dokterNaam,
            DateOnly datum,
            TimeOnly tijd)
        {
            Afspraak? afspraak = ZoekAfspraak(patientVoornaam, patientAchternaam, dokterNaam, datum, tijd);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }

            return DownloadDoktersattestVoorAfspraak(afspraak.Id);
        }

        public ResultaatDto DownloadDoktersattestVoorAfspraak(int afspraakId)
        {
            Doktersattest? doktersattest = _doktersattestRepository.ZoekOpAfspraakId(afspraakId);

            if (doktersattest == null)
            {
                return ResultaatDto.Mislukt("Er bestaat geen doktersattest voor deze afspraak.");
            }

            if (!doktersattest.IsVrijgegeven)
            {
                return ResultaatDto.Mislukt("Het doktersattest is nog niet vrijgegeven.");
            }

            if (doktersattest.IsGedownload)
            {
                return ResultaatDto.Mislukt("Het doktersattest werd al gedownload.");
            }

            doktersattest.IsGedownload = true;
            _doktersattestRepository.WerkBij(doktersattest);

            return ResultaatDto.Succes("Het doktersattest werd gedownload.");
        }

        public DoktersattestDto? ZoekDoktersattest(
            string patientVoornaam,
            string patientAchternaam,
            string dokterNaam,
            DateOnly datum,
            TimeOnly tijd)
        {
            Afspraak? afspraak = ZoekAfspraak(patientVoornaam, patientAchternaam, dokterNaam, datum, tijd);

            if (afspraak == null)
            {
                return null;
            }

            return ZoekDoktersattestOpAfspraakId(afspraak.Id);
        }

        public DoktersattestDto? ZoekDoktersattestOpAfspraakId(int afspraakId)
        {
            Doktersattest? doktersattest = _doktersattestRepository.ZoekOpAfspraakId(afspraakId);

            if (doktersattest == null)
            {
                return null;
            }

            return new DoktersattestDto
            {
                Id = doktersattest.Id,
                AfspraakId = doktersattest.AfspraakId,
                IsVrijgegeven = doktersattest.IsVrijgegeven,
                IsGedownload = doktersattest.IsGedownload
            };
        }

        private Afspraak? ZoekAfspraak(
            string patientVoornaam,
            string patientAchternaam,
            string dokterNaam,
            DateOnly datum,
            TimeOnly tijd)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientVoornaam, patientAchternaam);

            if (patient == null)
            {
                return null;
            }

            Dokter? dokter = _dokterRepository.ZoekOpNaam(dokterNaam);

            if (dokter == null)
            {
                return null;
            }

            return _afspraakRepository.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                datum,
                tijd);
        }
    }
}