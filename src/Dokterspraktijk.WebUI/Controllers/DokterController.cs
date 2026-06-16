using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.WebUI.ViewModels;
using Dokterspraktijk.WebUI.ViewModels.Dokter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dokterspraktijk.WebUI.Controllers
{
    [Authorize(Policy = "DokterOnly")]
    public class DokterController : Controller
    {
        private readonly IAfspraakService _afspraakService;
        private readonly IDoktersattestService _doktersattestService;
        private readonly IUnitOfWork _unitOfWork;

        public DokterController(
            IAfspraakService afspraakService,
            IDoktersattestService doktersattestService,
            IUnitOfWork unitOfWork)
        {
            _afspraakService = afspraakService;
            _doktersattestService = doktersattestService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index(DateOnly? datum)
        {
            string dokterNaam = GeefDokterNaamVoorIngelogdeGebruiker();

            DateOnly vandaag = DateOnly.FromDateTime(DateTime.Today);
            DateOnly geselecteerdeDatum = datum ?? vandaag;

            List<AfspraakDto> afsprakenVoorDokter = _afspraakService.GeefAlleAfspraken()
                .Where(afspraak => afspraak.DokterNaam == dokterNaam)
                .OrderBy(afspraak => afspraak.Datum)
                .ThenBy(afspraak => afspraak.Tijd)
                .ToList();

            List<AfspraakDto> afsprakenVandaag = afsprakenVoorDokter
                .Where(afspraak =>
                    afspraak.Datum == vandaag &&
                    afspraak.Status == "Gepland")
                .ToList();

            List<AfspraakDto> komendeAfspraken = afsprakenVoorDokter
                .Where(afspraak =>
                    afspraak.Datum > vandaag &&
                    afspraak.Datum <= vandaag.AddDays(7) &&
                    afspraak.Status == "Gepland")
                .ToList();

            List<AfspraakDto> afsprakenOpGeselecteerdeDatum = afsprakenVoorDokter
                .Where(afspraak => afspraak.Datum == geselecteerdeDatum)
                .OrderBy(afspraak => afspraak.Tijd)
                .ToList();

            DokterDashboardViewModel viewModel = new DokterDashboardViewModel
            {
                DokterNaam = dokterNaam,
                Vandaag = vandaag,
                GeselecteerdeDatum = geselecteerdeDatum,
                AfsprakenVandaag = afsprakenVandaag,
                KomendeAfspraken = komendeAfspraken,
                AfsprakenOpGeselecteerdeDatum = afsprakenOpGeselecteerdeDatum,
                KalenderDagen = MaakKalenderDagen(
                    geselecteerdeDatum.Year,
                    geselecteerdeDatum.Month,
                    geselecteerdeDatum,
                    afsprakenVoorDokter)
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            AfspraakDto? afspraak = _afspraakService.ZoekAfspraakOpId(id);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            string dokterNaam = GeefDokterNaamVoorIngelogdeGebruiker();

            if (afspraak.DokterNaam != dokterNaam)
            {
                return Forbid();
            }

            DokterAfspraakDetailsViewModel viewModel = new DokterAfspraakDetailsViewModel
            {
                Afspraak = afspraak,
                Doktersattest = _doktersattestService.ZoekDoktersattestOpAfspraakId(afspraak.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GeefDoktersattestVrij(int id)
        {
            AfspraakDto? afspraak = _afspraakService.ZoekAfspraakOpId(id);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            string dokterNaam = GeefDokterNaamVoorIngelogdeGebruiker();

            if (afspraak.DokterNaam != dokterNaam)
            {
                return Forbid();
            }

            _doktersattestService.GeefDoktersattestVrijVoorAfspraak(id);

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Afronden(int id)
        {
            AfspraakDto? afspraak = _afspraakService.ZoekAfspraakOpId(id);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            string dokterNaam = GeefDokterNaamVoorIngelogdeGebruiker();

            if (afspraak.DokterNaam != dokterNaam)
            {
                return Forbid();
            }

            _afspraakService.RondConsultatieAf(
                afspraak.DokterNaam,
                afspraak.PatientVoornaam,
                afspraak.PatientAchternaam,
                afspraak.Datum,
                afspraak.Tijd);

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Annuleren(int id)
        {
            AfspraakDto? afspraak = _afspraakService.ZoekAfspraakOpId(id);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            string dokterNaam = GeefDokterNaamVoorIngelogdeGebruiker();

            if (afspraak.DokterNaam != dokterNaam)
            {
                return Forbid();
            }

            _afspraakService.AnnuleerAfspraakOpId(id);

            return RedirectToAction(nameof(Afspraken));
        }

        [HttpGet]
        public IActionResult Patienten(string? zoekterm)
        {
            List<Patient> patienten = _unitOfWork.Patienten.GeefAllePatienten();

            if (!string.IsNullOrWhiteSpace(zoekterm))
            {
                string zoektermLower = zoekterm.ToLower();

                patienten = patienten
                    .Where(patient =>
                        patient.Voornaam.ToLower().Contains(zoektermLower) ||
                        patient.Achternaam.ToLower().Contains(zoektermLower) ||
                        patient.Email.ToLower().Contains(zoektermLower) ||
                        patient.Rijksregisternummer.ToLower().Contains(zoektermLower))
                    .ToList();
            }

            DokterPatientenViewModel viewModel = new DokterPatientenViewModel
            {
                Zoekterm = zoekterm,
                Patienten = patienten
                    .Select(patient => new PatientOverzichtViewModel
                    {
                        Id = patient.Id,
                        Voornaam = patient.Voornaam,
                        Achternaam = patient.Achternaam,
                        Email = patient.Email,
                        Telefoonnummer = patient.Telefoonnummer,
                        Rijksregisternummer = patient.Rijksregisternummer
                    })
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Afspraken(string? status)
        {
            string dokterNaam = GeefDokterNaamVoorIngelogdeGebruiker();

            List<AfspraakDto> afspraken = _afspraakService.GeefAlleAfspraken()
                .Where(afspraak => afspraak.DokterNaam == dokterNaam)
                .OrderBy(afspraak => afspraak.Datum)
                .ThenBy(afspraak => afspraak.Tijd)
                .ToList();

            if (!string.IsNullOrWhiteSpace(status))
            {
                afspraken = afspraken
                    .Where(afspraak => afspraak.Status == status)
                    .ToList();
            }

            DokterAfsprakenViewModel viewModel = new DokterAfsprakenViewModel
            {
                GekozenStatus = status,
                Afspraken = afspraken
            };

            return View(viewModel);
        }

        private string GeefDokterNaamVoorIngelogdeGebruiker()
        {
            string? dokterNaam = User.FindFirst("DokterNaam")?.Value;

            if (string.IsNullOrWhiteSpace(dokterNaam))
            {
                return "Timmermans";
            }

            return dokterNaam;
        }

        private List<DokterKalenderDagViewModel> MaakKalenderDagen(
            int jaar,
            int maand,
            DateOnly geselecteerdeDatum,
            List<AfspraakDto> afspraken)
        {
            List<DokterKalenderDagViewModel> kalenderDagen = new List<DokterKalenderDagViewModel>();

            DateOnly eersteDagVanMaand = new DateOnly(jaar, maand, 1);

            int dagenTerugTotMaandag = ((int)eersteDagVanMaand.DayOfWeek + 6) % 7;
            DateOnly startDatum = eersteDagVanMaand.AddDays(-dagenTerugTotMaandag);

            DateOnly vandaag = DateOnly.FromDateTime(DateTime.Today);

            for (int teller = 0; teller < 42; teller++)
            {
                DateOnly datum = startDatum.AddDays(teller);

                int aantalAfspraken = afspraken.Count(afspraak => afspraak.Datum == datum);

                kalenderDagen.Add(new DokterKalenderDagViewModel
                {
                    Datum = datum,
                    IsBinnenMaand = datum.Month == maand,
                    IsVandaag = datum == vandaag,
                    IsGeselecteerd = datum == geselecteerdeDatum,
                    HeeftAfspraken = aantalAfspraken > 0,
                    AantalAfspraken = aantalAfspraken
                });
            }

            return kalenderDagen;
        }
    }
}