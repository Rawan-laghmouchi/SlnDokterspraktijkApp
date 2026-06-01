using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;

namespace Dokterspraktijk.Application.Services.Implementation
{
    public class AfspraakService : IAfspraakService
    {
        private IPatientRepository _patientRepository;
        private IDokterRepository _dokterRepository;
        private ITijdslotRepository _tijdslotRepository;
        private IAfspraakRepository _afspraakRepository;

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

        public ResultaatDto MaakAfspraak(
            string patientVoornaam,
            string patientAchternaam,
            string email,
            string telefoonnummer,
            string rijksregisternummer,
            string dokterNaam,
            DateOnly datum,
            TimeOnly tijd,
            string reden)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientVoornaam, patientAchternaam);

            if (patient == null)
            {
                patient = new Patient
                {
                    Voornaam = patientVoornaam,
                    Achternaam = patientAchternaam,
                    Email = email,
                    Telefoonnummer = telefoonnummer,
                    Rijksregisternummer = rijksregisternummer
                };

                _patientRepository.VoegToe(patient);
            }
            else
            {
                patient.Email = email;
                patient.Telefoonnummer = telefoonnummer;
                patient.Rijksregisternummer = rijksregisternummer;

                _patientRepository.WerkBij(patient);
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
                tijdslot = new Tijdslot
                {
                    DokterId = dokter.Id,
                    Datum = datum,
                    Tijd = tijd,
                    Status = TijdslotStatus.Beschikbaar
                };

                _tijdslotRepository.VoegToe(tijdslot);
            }

            if (tijdslot.Status != TijdslotStatus.Beschikbaar)
            {
                return ResultaatDto.Mislukt("Het gekozen tijdslot is niet beschikbaar.");
            }

            Afspraak afspraak = new Afspraak(patient.Id, dokter.Id, tijdslot.Id, reden);

            _afspraakRepository.VoegToe(afspraak);

            tijdslot.Status = TijdslotStatus.NietBeschikbaar;
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

            afspraak.FotoBestandsnaam = fotoBestandsnaam;
            _afspraakRepository.WerkBij(afspraak);

            return ResultaatDto.Succes("De foto werd toegevoegd aan de afspraak.");
        }

        public List<AfspraakDto> GeefKomendeAfspraken(
            string patientVoornaam,
            string patientAchternaam,
            DateOnly vanafDatum)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientVoornaam, patientAchternaam);

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

                if (afspraak.Status != AfspraakStatus.Geannuleerd &&
                    tijdslot != null &&
                    dokter != null &&
                    tijdslot.Datum >= vanafDatum)
                {
                    AfspraakDto afspraakDto = new AfspraakDto
                    {
                        Id = afspraak.Id,
                        PatientVoornaam = patient.Voornaam,
                        PatientAchternaam = patient.Achternaam,
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

        public ResultaatDto AnnuleerAfspraak(
            string patientVoornaam,
            string patientAchternaam,
            string dokterNaam,
            DateOnly datum,
            TimeOnly tijd)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientVoornaam, patientAchternaam);

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

            if (afspraak.Status == AfspraakStatus.Afgerond)
            {
                return ResultaatDto.Mislukt("Een afgeronde afspraak kan niet geannuleerd worden.");
            }

            afspraak.Status = AfspraakStatus.Geannuleerd;
            _afspraakRepository.WerkBij(afspraak);

            Tijdslot? tijdslot = _tijdslotRepository.ZoekOpId(afspraak.TijdslotId);

            if (tijdslot != null)
            {
                tijdslot.Status = TijdslotStatus.Beschikbaar;
                _tijdslotRepository.WerkBij(tijdslot);
            }

            return ResultaatDto.Succes("De afspraak werd geannuleerd.");
        }

        public ResultaatDto RondConsultatieAf(
            string dokterNaam,
            string patientVoornaam,
            string patientAchternaam,
            DateOnly datum,
            TimeOnly tijd)
        {
            Patient? patient = _patientRepository.ZoekOpNaam(patientVoornaam, patientAchternaam);

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

            if (afspraak.Status == AfspraakStatus.Geannuleerd ||
                afspraak.Status == AfspraakStatus.Afgerond)
            {
                return ResultaatDto.Mislukt("Deze afspraak kan niet afgerond worden.");
            }

            afspraak.Status = AfspraakStatus.Afgerond;
            _afspraakRepository.WerkBij(afspraak);

            return ResultaatDto.Succes("De consultatie werd afgerond.");
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
        public List<AfspraakDto> GeefAlleAfspraken()
        {
            List<Afspraak> afspraken = _afspraakRepository.GeefAlleAfspraken();
            List<AfspraakDto> resultaat = new List<AfspraakDto>();

            foreach (Afspraak afspraak in afspraken)
            {
                Patient? patient = _patientRepository.ZoekOpId(afspraak.PatientId);
                Dokter? dokter = _dokterRepository.ZoekOpId(afspraak.DokterId);
                Tijdslot? tijdslot = _tijdslotRepository.ZoekOpId(afspraak.TijdslotId);

                if (patient != null && dokter != null && tijdslot != null)
                {
                    AfspraakDto afspraakDto = new AfspraakDto
                    {
                        Id = afspraak.Id,
                        PatientVoornaam = patient.Voornaam,
                        PatientAchternaam = patient.Achternaam,
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
        public AfspraakDto? ZoekAfspraak(
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

            Afspraak? afspraak = _afspraakRepository.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                datum,
                tijd);

            if (afspraak == null)
            {
                return null;
            }

            Tijdslot? tijdslot = _tijdslotRepository.ZoekOpId(afspraak.TijdslotId);

            if (tijdslot == null)
            {
                return null;
            }

            AfspraakDto afspraakDto = new AfspraakDto
            {
                Id = afspraak.Id,
                PatientVoornaam = patient.Voornaam,
                PatientAchternaam = patient.Achternaam,
                DokterNaam = dokter.Naam,
                Datum = tijdslot.Datum,
                Tijd = tijdslot.Tijd,
                Reden = afspraak.Reden,
                Status = VertaalAfspraakStatus(afspraak.Status),
                FotoBestandsnaam = afspraak.FotoBestandsnaam
            };

            return afspraakDto;
        }

        public AfspraakDto? ZoekAfspraakOpId(int id)
        {
            Afspraak? afspraak = _afspraakRepository.ZoekOpId(id);

            if (afspraak == null)
            {
                return null;
            }

            Patient? patient = _patientRepository.ZoekOpId(afspraak.PatientId);
            Dokter? dokter = _dokterRepository.ZoekOpId(afspraak.DokterId);
            Tijdslot? tijdslot = _tijdslotRepository.ZoekOpId(afspraak.TijdslotId);

            if (patient == null || dokter == null || tijdslot == null)
            {
                return null;
            }

            AfspraakDto afspraakDto = new AfspraakDto
            {
                Id = afspraak.Id,
                PatientVoornaam = patient.Voornaam,
                PatientAchternaam = patient.Achternaam,
                DokterNaam = dokter.Naam,
                Datum = tijdslot.Datum,
                Tijd = tijdslot.Tijd,
                Reden = afspraak.Reden,
                Status = VertaalAfspraakStatus(afspraak.Status),
                FotoBestandsnaam = afspraak.FotoBestandsnaam
            };

            return afspraakDto;
        }
    }
}