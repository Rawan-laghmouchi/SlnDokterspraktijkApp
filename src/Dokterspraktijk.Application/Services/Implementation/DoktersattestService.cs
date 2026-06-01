using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Services.Implementation
{
    public class DoktersattestService : IDoktersattestService
    {
        private IPatientRepository _patientRepository;
        private IDokterRepository _dokterRepository;
        private IAfspraakRepository _afspraakRepository;
        private IDoktersattestRepository _doktersattestRepository;

        public DoktersattestService(IPatientRepository patientRepository, IDokterRepository dokterRepository, IAfspraakRepository afspraakRepository, IDoktersattestRepository doktersattestRepository)
        {
            _patientRepository = patientRepository;
            _dokterRepository = dokterRepository;
            _afspraakRepository = afspraakRepository;
            _doktersattestRepository = doktersattestRepository;
        }
        public ResultaatDto GeefDoktersattestVrij(string dokterNaam, string patientVoornaam, string patientAchternaam, DateOnly datum, TimeOnly tijd)
        {
            Afspraak? afspraak = ZoekAfspraak(patientVoornaam, patientAchternaam, dokterNaam, datum, tijd);

            if (afspraak is null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }

            if (afspraak.Status != AfspraakStatus.Afgerond)
            {
                return ResultaatDto.Mislukt("Een doktersattest kan pas na een afgeronde consultatie worden vrijgegeven.");
            }

            Doktersattest? bestaandDoktersattest = _doktersattestRepository.ZoekOpAfspraakId(afspraak.Id);

            if (bestaandDoktersattest is null)
            {
                int nieuwId = afspraak.Id;

                Doktersattest nieuwDoktersattest = new Doktersattest(nieuwId, afspraak.Id);
                nieuwDoktersattest.IsVrijgegeven = true;

                _doktersattestRepository.VoegToe(nieuwDoktersattest);
            }
            else
            {
                bestaandDoktersattest.IsVrijgegeven = true;
                _doktersattestRepository.WerkBij(bestaandDoktersattest);
            }

            return ResultaatDto.Succes("Het doktersattest werd vrijgegeven.");
        }

        public ResultaatDto DownloadDoktersattest(string patientVoornaam, string patientAchternaam, string dokterNaam, DateOnly datum, TimeOnly tijd)
        {
            Afspraak? afspraak = ZoekAfspraak(patientVoornaam, patientAchternaam, dokterNaam, datum, tijd);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }

            Doktersattest? doktersattest = _doktersattestRepository.ZoekOpAfspraakId(afspraak.Id);

            if (doktersattest == null)
            {
                return ResultaatDto.Mislukt("Er bestaat geen doktersattest voor deze afspraak.");
            }

            if (!doktersattest.IsVrijgegeven)
            {
                return ResultaatDto.Mislukt("Het doktersattest is nog niet vrijgegeven.");
            }

            doktersattest.IsGedownload = true;
            _doktersattestRepository.WerkBij(doktersattest);

            return ResultaatDto.Succes("Het doktersattest werd gedownload.");
        }

        public DoktersattestDto? ZoekDoktersattest(string patientVoornaam, string patientAchternaam, string dokterNaam, DateOnly datum, TimeOnly tijd)
        {
            Afspraak? afspraak = ZoekAfspraak(patientVoornaam, patientAchternaam, dokterNaam, datum, tijd);

            if (afspraak == null)
            {
                return null;
            }

            Doktersattest? doktersattest = _doktersattestRepository.ZoekOpAfspraakId(afspraak.Id);

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

        private Afspraak? ZoekAfspraak(string patientVoornaam, string patientAchternaam, string dokterNaam, DateOnly datum, TimeOnly tijd)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientVoornaam, patientAchternaam);

            if (patient is null)
            {
                return null;
            }

            Dokter? dokter = _dokterRepository.ZoekOpNaam(dokterNaam);

            if (dokter is null)
            {
                return null;
            }

            return _afspraakRepository.ZoekOpPatientDokterDatumEnTijd(patient.Id, dokter.Id, datum, tijd);
        }
    }
}
