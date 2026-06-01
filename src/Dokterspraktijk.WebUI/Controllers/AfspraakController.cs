using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Enums;
using Dokterspraktijk.WebUI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dokterspraktijk.WebUI.Controllers
{
    public class AfspraakController : Controller
    {
        private IAfspraakService _afspraakService;
        private ITijdslotService _tijdslotService;
        private IDokterRepository _dokterRepository;
        private IAfspraakCategorieRepository _afspraakCategorieRepository;

        public AfspraakController(
        IAfspraakService afspraakService,
        ITijdslotService tijdslotService,
        IDokterRepository dokterRepository,
        IAfspraakCategorieRepository afspraakCategorieRepository)
        {
            _afspraakService = afspraakService;
            _tijdslotService = tijdslotService;
            _dokterRepository = dokterRepository;
            _afspraakCategorieRepository = afspraakCategorieRepository;
        }

        [HttpGet]
        public IActionResult Create(string? dokterNaam, DateOnly? datum)
        {
            AfspraakCreateViewModel model = new AfspraakCreateViewModel();

            model.DokterNaam = dokterNaam ?? string.Empty;
            model.Datum = datum ?? DateOnly.FromDateTime(DateTime.Today);

            VulKeuzelijsten(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AfspraakCreateViewModel model)
        {
            VulKeuzelijsten(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Datum < DateOnly.FromDateTime(DateTime.Today))
            {
                return View(model);
            }

            if (model.Datum.DayOfWeek == DayOfWeek.Saturday ||
                model.Datum.DayOfWeek == DayOfWeek.Sunday)
            {
                return View(model);
            }

            if (model.Tijd == default)
            {
                return View(model);
            }

            ResultaatDto resultaat = _afspraakService.MaakAfspraak(
                model.PatientVoornaam,
                model.PatientAchternaam,
                model.Email,
                model.Telefoonnummer,
                model.Rijksregisternummer,
                model.DokterNaam,
                model.Datum,
                model.Tijd,
                model.Reden);
            if (!resultaat.IsGelukt)
            {
                ModelState.AddModelError(string.Empty, resultaat.Melding);
                return View(model);
            }

            AfspraakDto? afspraak = _afspraakService.ZoekAfspraak(
                model.PatientVoornaam,
                model.PatientAchternaam,
                model.DokterNaam,
                model.Datum,
                model.Tijd);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Bevestiging), new { id = afspraak.Id });
        }

        [HttpGet]
        public IActionResult Bevestiging(int id)
        {
            AfspraakDto? afspraak = _afspraakService.ZoekAfspraakOpId(id);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(afspraak);
        }
        private void VulKeuzelijsten(AfspraakCreateViewModel model)
        {
            model.Dokters = _dokterRepository.GeefAlleDokters()
                .Select(dokter => new DokterKeuzeViewModel
                {
                    Naam = dokter.Naam,
                    Specialisatie = dokter.Specialisatie
                })
                .ToList();

            if (string.IsNullOrWhiteSpace(model.DokterNaam) && model.Dokters.Any())
            {
                model.DokterNaam = model.Dokters.First().Naam;
            }

            model.Tijdsloten = new List<TijdslotKeuzeViewModel>();

            if (string.IsNullOrWhiteSpace(model.DokterNaam))
            {
                return;
            }

            if (model.Datum.DayOfWeek == DayOfWeek.Saturday ||
                model.Datum.DayOfWeek == DayOfWeek.Sunday)
            {
                return;
            }

            List<TijdslotDto> bestaandeTijdsloten = _tijdslotService.RaadpleegTijdsloten(
                model.DokterNaam,
                model.Datum);

            TimeOnly tijd = new TimeOnly(8, 30);
            TimeOnly eindTijd = new TimeOnly(17, 30);

            while (tijd <= eindTijd)
            {
                TijdslotDto? bestaandTijdslot = bestaandeTijdsloten
                    .FirstOrDefault(tijdslot => tijdslot.Tijd == tijd);

                bool tijdslotMagGetoondWorden = true;

                if (bestaandTijdslot != null &&
                    bestaandTijdslot.Status != "beschikbaar")
                {
                    tijdslotMagGetoondWorden = false;
                }

                if (tijdslotMagGetoondWorden)
                {
                    model.Tijdsloten.Add(new TijdslotKeuzeViewModel
                    {
                        Tijd = tijd,
                        IsBeschikbaar = true
                    });
                }

                tijd = tijd.AddMinutes(30);
            }
            model.AfspraakCategorieen = _afspraakCategorieRepository.GeefAlleCategorieen()
                .Select(categorie => new AfspraakCategorieViewModel
            {
                Id = categorie.Id,
                Naam = categorie.Naam
            })
            .ToList();
        }

        public IActionResult Index(string? gekozenStatus)
        {
            List<AfspraakDto> afspraken = _afspraakService.GeefAlleAfspraken();

            if (!string.IsNullOrWhiteSpace(gekozenStatus))
            {
                afspraken = afspraken
                    .Where(afspraak => afspraak.Status == gekozenStatus)
                    .ToList();
            }

            List<SelectListItem> statusOpties = Enum.GetValues<AfspraakStatus>()
                .Select(status => new SelectListItem
                {
                    Value = status.ToString(),
                    Text = status.ToString()
                })
                .ToList();

            AfspraakIndexViewModel viewModel = new AfspraakIndexViewModel
            {
                Afspraken = afspraken,
                GekozenStatus = gekozenStatus,
                StatusOpties = statusOpties
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Annuleer(int id)
        {
            AfspraakDto? afspraak = _afspraakService.ZoekAfspraakOpId(id);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(afspraak);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AnnuleerBevestigd(int id)
        {
            AfspraakDto? afspraak = _afspraakService.ZoekAfspraakOpId(id);

            ResultaatDto resultaat = _afspraakService.AnnuleerAfspraak(
                afspraak.PatientVoornaam,
                afspraak.PatientAchternaam,
                afspraak.DokterNaam,
                afspraak.Datum,
                afspraak.Tijd);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}