using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;

namespace Dokterspraktijk.Application.Services.Implementation
{
    public class AfspraakService : IAfspraakService
    {
        private IUnitOfWork _unitOfWork;

        public AfspraakService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            Patient? patient = _unitOfWork.Patienten.ZoekOpEmail(email);

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

                _unitOfWork.Patienten.VoegToe(patient);
            }
            else
            {
                patient.Voornaam = patientVoornaam;
                patient.Achternaam = patientAchternaam;
                patient.Email = email;
                patient.Telefoonnummer = telefoonnummer;
                patient.Rijksregisternummer = rijksregisternummer;

                _unitOfWork.Patienten.WerkBij(patient);
            }

            Dokter? dokter = _unitOfWork.Dokters.ZoekOpNaam(dokterNaam);

            if (dokter == null)
            {
                return ResultaatDto.Mislukt("De dokter werd niet gevonden.");
            }

            Tijdslot? tijdslot = _unitOfWork.Tijdsloten.ZoekVoorDokterOpDatumEnTijd(
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

                _unitOfWork.Tijdsloten.VoegToe(tijdslot);
            }

            if (tijdslot.Status != TijdslotStatus.Beschikbaar)
            {
                return ResultaatDto.Mislukt("Het gekozen tijdslot is niet beschikbaar.");
            }

            Afspraak afspraak = new Afspraak
            {
                PatientId = patient.Id,
                DokterId = dokter.Id,
                TijdslotId = tijdslot.Id,
                Reden = reden,
                Status = AfspraakStatus.Gepland
            };

            _unitOfWork.Afspraken.VoegToe(afspraak);

            tijdslot.Status = TijdslotStatus.NietBeschikbaar;
            _unitOfWork.Tijdsloten.WerkBij(tijdslot);

            return ResultaatDto.Succes("De afspraak werd aangemaakt.");
        }

        public ResultaatDto VoegFotoToeAanAfspraak(int afspraakId, string fotoBestandsnaam)
        {
            Afspraak? afspraak = _unitOfWork.Afspraken.ZoekOpId(afspraakId);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }

            afspraak.FotoBestandsnaam = fotoBestandsnaam;
            _unitOfWork.Afspraken.WerkBij(afspraak);

            return ResultaatDto.Succes("De foto werd toegevoegd aan de afspraak.");
        }

        public List<AfspraakDto> GeefKomendeAfspraken(
            string patientVoornaam,
            string patientAchternaam,
            DateOnly vanafDatum)
        {
            Patient? patient = _unitOfWork.Patienten.ZoekOpNaam(patientVoornaam, patientAchternaam);

            if (patient == null)
            {
                return new List<AfspraakDto>();
            }

            List<Afspraak> afspraken = _unitOfWork.Afspraken.GeefAfsprakenVoorPatient(patient.Id);
            List<AfspraakDto> resultaat = new List<AfspraakDto>();

            foreach (Afspraak afspraak in afspraken)
            {
                Tijdslot? tijdslot = _unitOfWork.Tijdsloten.ZoekOpId(afspraak.TijdslotId);
                Dokter? dokter = _unitOfWork.Dokters.ZoekOpId(afspraak.DokterId);

                if (afspraak.Status != AfspraakStatus.Geannuleerd &&
                    tijdslot != null &&
                    dokter != null &&
                    tijdslot.Datum >= vanafDatum)
                {
                    AfspraakDto afspraakDto = MaakAfspraakDto(
                        afspraak,
                        patient,
                        dokter,
                        tijdslot);

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

            Afspraak? afspraak = _unitOfWork.Afspraken.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                datum,
                tijd);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }

            if (afspraak.Status != AfspraakStatus.Gepland)
            {
                return ResultaatDto.Mislukt("Alleen een geplande afspraak kan geannuleerd worden.");
            }

            afspraak.Status = AfspraakStatus.Geannuleerd;
            _unitOfWork.Afspraken.WerkBij(afspraak);

            Tijdslot? tijdslot = _unitOfWork.Tijdsloten.ZoekOpId(afspraak.TijdslotId);

            if (tijdslot != null)
            {
                tijdslot.Status = TijdslotStatus.Beschikbaar;
                _unitOfWork.Tijdsloten.WerkBij(tijdslot);
            }

            return ResultaatDto.Succes("De afspraak werd geannuleerd.");
        }
        public ResultaatDto AnnuleerAfspraakOpId(int afspraakId)
        {
            Afspraak? afspraak = _unitOfWork.Afspraken.ZoekOpId(afspraakId);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }

            if (afspraak.Status != AfspraakStatus.Gepland)
            {
                return ResultaatDto.Mislukt("Alleen een geplande afspraak kan geannuleerd worden.");
            }

            afspraak.Status = AfspraakStatus.Geannuleerd;
            _unitOfWork.Afspraken.WerkBij(afspraak);

            Tijdslot? tijdslot = _unitOfWork.Tijdsloten.ZoekOpId(afspraak.TijdslotId);

            if (tijdslot != null)
            {
                tijdslot.Status = TijdslotStatus.Beschikbaar;
                _unitOfWork.Tijdsloten.WerkBij(tijdslot);
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

            Afspraak? afspraak = _unitOfWork.Afspraken.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                datum,
                tijd);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }

            if (afspraak.Status != AfspraakStatus.Gepland)
            {
                return ResultaatDto.Mislukt("Alleen een geplande afspraak kan afgerond worden.");
            }

            afspraak.Status = AfspraakStatus.Afgerond;
            _unitOfWork.Afspraken.WerkBij(afspraak);

            return ResultaatDto.Succes("De consultatie werd afgerond.");
        }

        public ResultaatDto ValideerBestandVoorAfspraakaanvraag(int afspraakId, string bestandsnaam)
        {
            Afspraak? afspraak = _unitOfWork.Afspraken.ZoekOpId(afspraakId);

            if (afspraak == null)
            {
                return ResultaatDto.Mislukt("De afspraak werd niet gevonden.");
            }

            string extensie = Path.GetExtension(bestandsnaam).ToLowerInvariant();

            if (extensie == ".jpg" ||
                extensie == ".jpeg" ||
                extensie == ".png" ||
                extensie == ".pdf")
            {
                return ResultaatDto.Succes("Het bestand werd geaccepteerd.");
            }

            return ResultaatDto.Mislukt("Het bestandstype wordt niet ondersteund.");
        }

        public List<AfspraakDto> GeefAlleAfspraken()
        {
            List<Afspraak> afspraken = _unitOfWork.Afspraken.GeefAlleAfspraken();
            List<AfspraakDto> resultaat = new List<AfspraakDto>();

            foreach (Afspraak afspraak in afspraken)
            {
                Patient? patient = _unitOfWork.Patienten.ZoekOpId(afspraak.PatientId);
                Dokter? dokter = _unitOfWork.Dokters.ZoekOpId(afspraak.DokterId);
                Tijdslot? tijdslot = _unitOfWork.Tijdsloten.ZoekOpId(afspraak.TijdslotId);

                if (patient != null && dokter != null && tijdslot != null)
                {
                    AfspraakDto afspraakDto = MaakAfspraakDto(
                        afspraak,
                        patient,
                        dokter,
                        tijdslot);

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
            Patient? patient = _unitOfWork.Patienten.ZoekOpNaam(patientVoornaam, patientAchternaam);

            if (patient == null)
            {
                return null;
            }

            Dokter? dokter = _unitOfWork.Dokters.ZoekOpNaam(dokterNaam);

            if (dokter == null)
            {
                return null;
            }

            Afspraak? afspraak = _unitOfWork.Afspraken.ZoekOpPatientDokterDatumEnTijd(
                patient.Id,
                dokter.Id,
                datum,
                tijd);

            if (afspraak == null)
            {
                return null;
            }

            Tijdslot? tijdslot = _unitOfWork.Tijdsloten.ZoekOpId(afspraak.TijdslotId);

            if (tijdslot == null)
            {
                return null;
            }

            return MaakAfspraakDto(
                afspraak,
                patient,
                dokter,
                tijdslot);
        }

        public AfspraakDto? ZoekAfspraakOpId(int id)
        {
            Afspraak? afspraak = _unitOfWork.Afspraken.ZoekOpId(id);

            if (afspraak == null)
            {
                return null;
            }

            Patient? patient = _unitOfWork.Patienten.ZoekOpId(afspraak.PatientId);
            Dokter? dokter = _unitOfWork.Dokters.ZoekOpId(afspraak.DokterId);
            Tijdslot? tijdslot = _unitOfWork.Tijdsloten.ZoekOpId(afspraak.TijdslotId);

            if (patient == null || dokter == null || tijdslot == null)
            {
                return null;
            }

            return MaakAfspraakDto(
                afspraak,
                patient,
                dokter,
                tijdslot);
        }

        private static AfspraakDto MaakAfspraakDto(
            Afspraak afspraak,
            Patient patient,
            Dokter dokter,
            Tijdslot tijdslot)
        {
            return new AfspraakDto
            {
                Id = afspraak.Id,
                PatientVoornaam = patient.Voornaam,
                PatientAchternaam = patient.Achternaam,
                PatientEmail = patient.Email,
                PatientTelefoonnummer = patient.Telefoonnummer,
                PatientRijksregisternummer = patient.Rijksregisternummer,
                DokterNaam = dokter.Naam,
                Datum = tijdslot.Datum,
                Tijd = tijdslot.Tijd,
                Reden = afspraak.Reden,
                Status = VertaalAfspraakStatus(afspraak.Status),
                FotoBestandsnaam = afspraak.FotoBestandsnaam
            };
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
    }
}