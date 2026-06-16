using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;
using Dokterspraktijk.Infrastructure.Identity;
using Dokterspraktijk.WebUI.ViewModels.Afspraak;
using Dokterspraktijk.WebUI.ViewModels.Dokter;
using Dokterspraktijk.WebUI.ViewModels.Tijdslot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace Dokterspraktijk.WebUI.Controllers
{
    [Authorize(Policy = "PatientOnly")]
    public class AfspraakController : Controller
    {
        private IAfspraakService _afspraakService;
        private ITijdslotService _tijdslotService;
        private IDoktersattestService _doktersattestService;
        private UserManager<ApplicationUser> _userManager;
        private IUnitOfWork _unitOfWork;

        public AfspraakController(
            IAfspraakService afspraakService,
            ITijdslotService tijdslotService,
            IDoktersattestService doktersattestService,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _afspraakService = afspraakService;
            _tijdslotService = tijdslotService;
            _doktersattestService = doktersattestService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string? dokterNaam, DateOnly? datum)
        {
            AfspraakCreateViewModel model = new AfspraakCreateViewModel();

            model.DokterNaam = dokterNaam ?? string.Empty;
            model.Datum = datum ?? DateOnly.FromDateTime(DateTime.Today);

            await VulPatientGegevensIn(model);

            VulKeuzelijsten(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AfspraakCreateViewModel model)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User)
                ?? throw new InvalidOperationException("Een afspraak maken kan enkel met een account.");

            string email = user.Email
                ?? throw new InvalidOperationException("Het account heeft geen e-mailadres.");

            model.Email = email;

            ModelState.Remove(nameof(model.Email));
            ModelState.Remove(nameof(model.PatientGegevensZijnVoorafIngevuld));

            VulKeuzelijsten(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Datum < DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError(nameof(model.Datum), "Je kan geen afspraak maken in het verleden.");
                return View(model);
            }

            if (model.Datum.DayOfWeek == DayOfWeek.Saturday ||
                model.Datum.DayOfWeek == DayOfWeek.Sunday)
            {
                ModelState.AddModelError(nameof(model.Datum), "Je kan enkel een afspraak maken op een werkdag.");
                return View(model);
            }

            if (model.Tijd == default)
            {
                ModelState.AddModelError(nameof(model.Tijd), "Kies een beschikbaar tijdslot.");
                return View(model);
            }

            await WerkAccountGegevensBij(user, model);

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

            AfspraakDto? afspraak = _afspraakService.GeefAlleAfspraken()
             .Where(afspraak =>
                 string.Equals(afspraak.PatientEmail, model.Email, StringComparison.OrdinalIgnoreCase) &&
                 afspraak.DokterNaam == model.DokterNaam &&
                 afspraak.Datum == model.Datum &&
                 afspraak.Tijd == model.Tijd)
             .OrderByDescending(afspraak => afspraak.Id)
             .FirstOrDefault();

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Bevestiging), new { id = afspraak.Id });
        }
        [HttpGet]
        public async Task<IActionResult> Bevestiging(int id)
        {
            AfspraakDto? afspraak = _afspraakService.ZoekAfspraakOpId(id);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            string email = await GeefEmailVanIngelogdeGebruikerAsync();

            if (!string.Equals(afspraak.PatientEmail, email, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            return View(afspraak);
        }
        private void VulKeuzelijsten(AfspraakCreateViewModel model)
        {
            model.MinimumDatum = DateOnly
                .FromDateTime(DateTime.Today)
                .ToString("yyyy-MM-dd");

            model.Dokters = _unitOfWork.Dokters.GeefAlleDokters()
                .Select(dokter => new DokterKeuzeViewModel
                {
                    Naam = dokter.Naam,
                    Specialisatie = dokter.Specialisatie
                })
                .ToList();

            model.AfspraakCategorieen = _unitOfWork.AfspraakCategorieen.GeefAlleCategorieen()
                .Select(categorie => new AfspraakCategorieViewModel
                {
                    Id = categorie.Id,
                    Naam = categorie.Naam
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
                !string.Equals(bestaandTijdslot.Status, "beschikbaar", StringComparison.OrdinalIgnoreCase))
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
            
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? gekozenStatus)
        {
            string email = await GeefEmailVanIngelogdeGebruikerAsync();

            List<AfspraakDto> afspraken = _afspraakService.GeefAlleAfspraken()
                .Where(afspraak => string.Equals(afspraak.PatientEmail, email, StringComparison.OrdinalIgnoreCase))
                .ToList();

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

            Dictionary<int, DoktersattestDto?> doktersattestenPerAfspraakId = afspraken.ToDictionary(
                afspraak => afspraak.Id,
                afspraak => _doktersattestService.ZoekDoktersattestOpAfspraakId(afspraak.Id));

            AfspraakIndexViewModel viewModel = new AfspraakIndexViewModel
            {
                Afspraken = afspraken,
                GekozenStatus = gekozenStatus,
                StatusOpties = statusOpties,
                DoktersattestenPerAfspraakId = doktersattestenPerAfspraakId
            };

            return View(viewModel);
        }
        private async Task<string> GeefEmailVanIngelogdeGebruikerAsync()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User)
                ?? throw new InvalidOperationException("Geen account gevonden.");

            string email = user.Email
                ?? throw new InvalidOperationException("Het account heeft geen e-mailadres.");

            return email;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadDoktersattest(int id)
        {
            AfspraakDto? afspraak = _afspraakService.ZoekAfspraakOpId(id);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            string email = await GeefEmailVanIngelogdeGebruikerAsync();

            if (!string.Equals(afspraak.PatientEmail, email, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            ResultaatDto resultaat = _doktersattestService.DownloadDoktersattestVoorAfspraak(id);

            if (!resultaat.IsGelukt)
            {
                return RedirectToAction(nameof(Index));
            }

            string inhoud = MaakDoktersattestTekst(afspraak);
            byte[] bestand = Encoding.UTF8.GetBytes(inhoud);

            string bestandsnaam = $"doktersattest-{afspraak.Id}-{afspraak.Datum:yyyyMMdd}.txt";

            return File(bestand, "text/plain", bestandsnaam);
        }

        [HttpGet]
        public async Task<IActionResult> Annuleer(int id)
        {
            AfspraakDto? afspraak = _afspraakService.ZoekAfspraakOpId(id);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            string email = await GeefEmailVanIngelogdeGebruikerAsync();

            if (!string.Equals(afspraak.PatientEmail, email, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            return View(afspraak);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnnuleerBevestigd(int id)
        {
            AfspraakDto? afspraak = _afspraakService.ZoekAfspraakOpId(id);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            string email = await GeefEmailVanIngelogdeGebruikerAsync();

            if (!string.Equals(afspraak.PatientEmail, email, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            ResultaatDto resultaat = _afspraakService.AnnuleerAfspraak(
                afspraak.PatientVoornaam,
                afspraak.PatientAchternaam,
                afspraak.DokterNaam,
                afspraak.Datum,
                afspraak.Tijd);

            if (!resultaat.IsGelukt)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        private string MaakDoktersattestTekst(AfspraakDto afspraak)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("DOKTERSATTEST");
            builder.AppendLine();
            builder.AppendLine($"Patiënt: {afspraak.PatientVoornaam} {afspraak.PatientAchternaam}");
            builder.AppendLine($"Dokter: Dr. {afspraak.DokterNaam}");
            builder.AppendLine($"Datum consultatie: {afspraak.Datum:dd/MM/yyyy}");
            builder.AppendLine($"Tijdstip: {afspraak.Tijd:HH:mm}");
            builder.AppendLine($"Reden: {afspraak.Reden}");
            builder.AppendLine();
            builder.AppendLine("Dit attest werd digitaal vrijgegeven door de dokter.");
            builder.AppendLine("Dokterspraktijk Laghmouchi");

            return builder.ToString();
        }

        private async Task VulPatientGegevensIn(AfspraakCreateViewModel model)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User)
                ?? throw new InvalidOperationException("Een afspraak maken kan enkel met een account.");

            string email = user.Email
                ?? throw new InvalidOperationException("Het account heeft geen e-mailadres.");

            Patient? patient = _unitOfWork.Patienten.ZoekOpEmail(email);

            if (patient != null)
            {
                model.PatientVoornaam = patient.Voornaam;
                model.PatientAchternaam = patient.Achternaam;
                model.Email = patient.Email;
                model.Telefoonnummer = patient.Telefoonnummer;
                model.Rijksregisternummer = patient.Rijksregisternummer;
                model.PatientGegevensZijnVoorafIngevuld = true;

                return;
            }

            model.PatientVoornaam = user.Voornaam;
            model.PatientAchternaam = user.Achternaam;
            model.Email = email;
            model.Telefoonnummer = user.PhoneNumber ?? string.Empty;
            model.PatientGegevensZijnVoorafIngevuld = false;
        }
        private async Task WerkAccountGegevensBij(
            ApplicationUser user,
            AfspraakCreateViewModel model)
        {
            bool accountMoetBijgewerktWorden = false;

            if (user.Voornaam != model.PatientVoornaam)
            {
                user.Voornaam = model.PatientVoornaam;
                accountMoetBijgewerktWorden = true;
            }

            if (user.Achternaam != model.PatientAchternaam)
            {
                user.Achternaam = model.PatientAchternaam;
                accountMoetBijgewerktWorden = true;
            }

            if (user.PhoneNumber != model.Telefoonnummer)
            {
                user.PhoneNumber = model.Telefoonnummer;
                accountMoetBijgewerktWorden = true;
            }

            if (accountMoetBijgewerktWorden)
            {
                await _userManager.UpdateAsync(user);
            }
        }
    }
}