using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;

namespace Dokterspraktijk.Application.Services.Implementation
{
    public class AfspraakService : IAfspraakService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IDokterRepository _dokterRepository;
        private readonly ITijdslotRepository _tijdslotRepository;
        private readonly IAfspraakRepository _afspraakRepository;

        public AfspraakService(
            IPatientRepository patientRepository,
            IDokterRepository dokterRepository,
            ITijdslotRepository tijdslotRepository,
            IAfspraakRepository afspraakRepository)
        {
            _patientRepository = patientRepository;
            _dokterRepository = dokterRepository;
            _tijdslotRepository = tijdslotRepository;
            _afspraakRepository = afspraakRepository;
        }

        public ResultaatDto MaakAfspraak(string patientNaam, string dokterNaam, DateOnly datum, TimeOnly tijd, string reden)
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

            Tijdslot? tijdslot = _tijdslotRepository.ZoekVoorDokterOpDatumEnTijd(
                dokter.Id,
                datum,
                tijd);

            if (tijdslot == null)
            {
                return ResultaatDto.Mislukt("Het gekozen tijdslot bestaat niet.");
            }

            if (!tijdslot.IsBeschikbaar())
            {
                return ResultaatDto.Mislukt("Het gekozen tijdslot is niet beschikbaar.");
            }

            int afspraakId = _afspraakRepository.GeefAfsprakenVoorPatient(patient.Id).Count + 1;

            Afspraak afspraak = new Afspraak(
                afspraakId,
                patient.Id,
                dokter.Id,
                tijdslot.Id,
                reden);

            _afspraakRepository.VoegToe(afspraak);

            tijdslot.MaakNietBeschikbaar();
            _tijdslotRepository.WerkBij(tijdslot);

            return ResultaatDto.Succes("De afspraak werd aangemaakt.");
        }

        public ResultaatDto VoegFotoToeAanAfspraak(int afspraakId, string fotoBestandsnaam)
        {
            Afspraak? afspraak = _afspraakRepository.ZoekOpId(afspraakId);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }

            afspraak.VoegFotoToe(fotoBestandsnaam);
            _afspraakRepository.WerkBij(afspraak);

            return ResultaatDto.Succes("De foto werd toegevoegd aan de afspraak.");
        }

        public List<AfspraakDto> GeefKomendeAfspraken(string patientNaam, DateOnly vanafDatum)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientNaam);

            if (patient == null)
            {
                return new List<AfspraakDto>();
            }

            List<Afspraak> afspraken = _afspraakRepository.GeefAfsprakenVoorPatient(patient.Id);
            List<AfspraakDto> resultaat = new List<AfspraakDto>();

            foreach (Afspraak afspraak in afspraken)
            {
                Tijdslot? tijdslot = _tijdslotRepository.ZoekOpId(afspraak.TijdslotId);
                Dokter? dokter = _dokterRepository.ZoekOpId(afspraak.DokterId);

                if (!afspraak.IsGeannuleerd() &&
                    tijdslot != null &&
                    dokter != null &&
                    tijdslot.Datum >= vanafDatum)
                {
                    AfspraakDto afspraakDto = new AfspraakDto
                    {
                        Id = afspraak.Id,
                        PatientNaam = patient.Naam,
                        DokterNaam = dokter.Naam,
                        Datum = tijdslot.Datum,
                        Tijd = tijdslot.Tijd,
                        Reden = afspraak.Reden,
                        Status = VertaalAfspraakStatus(afspraak.Status),
                        FotoBestandsnaam = afspraak.FotoBestandsnaam
                    };

                    resultaat.Add(afspraakDto);
                }
            }

            return resultaat
                .OrderBy(afspraak => afspraak.Datum)
                .ThenBy(afspraak => afspraak.Tijd)
                .ToList();
        }

        public ResultaatDto AnnuleerAfspraak(string patientNaam, string dokterNaam, DateOnly datum, TimeOnly tijd)
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

            Afspraak? afspraak = _afspraakRepository.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                datum,
                tijd);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }
            if (!afspraak.KanGeannuleerdWorden())
            {
                return ResultaatDto.Mislukt("Een afgeronde afspraak kan niet geannuleerd worden.");
            }
            afspraak.Annuleer();
            _afspraakRepository.WerkBij(afspraak);

            Tijdslot? tijdslot = _tijdslotRepository.ZoekOpId(afspraak.TijdslotId);

            if (tijdslot != null)
            {
                tijdslot.MaakBeschikbaar();
                _tijdslotRepository.WerkBij(tijdslot);
            }

            return ResultaatDto.Succes("De afspraak werd geannuleerd.");
        }

        public ResultaatDto RondConsultatieAf(string dokterNaam, string patientNaam, DateOnly datum, TimeOnly tijd)
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

            Afspraak? afspraak = _afspraakRepository.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                datum,
                tijd);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }
            if (!afspraak.KanAfgerondWorden())
            {
                return ResultaatDto.Mislukt("Deze afspraak kan niet afgerond worden.");
            }

            afspraak.RondAf();
            _afspraakRepository.WerkBij(afspraak);

            return ResultaatDto.Succes("De consultatie werd afgerond.");
        }


        private static string VertaalAfspraakStatus(AfspraakStatus status)
        {
            if (status == AfspraakStatus.Gepland)
            {
                return "Gepland";
            }

            if (status == AfspraakStatus.Geannuleerd)
            {
                return "Geannuleerd";
            }

            return "Afgerond";
        }

        public ResultaatDto ValideerBestandVoorAfspraakaanvraag(int afspraakId, string bestandsnaam)
        {
            Afspraak? afspraak = _afspraakRepository.ZoekOpId(afspraakId);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }

            string extensie = Path.GetExtension(bestandsnaam).ToLowerInvariant();

            if (extensie == ".jpg" || extensie == ".jpeg" || extensie == ".png")
            {
                return ResultaatDto.Succes("Het bestand werd geaccepteerd.");
            }

            return ResultaatDto.Mislukt("Het bestandstype wordt niet ondersteund.");
        }
    }
}