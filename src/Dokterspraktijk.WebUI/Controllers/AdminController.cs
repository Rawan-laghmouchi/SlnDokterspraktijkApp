using Dokterspraktijk.Application.Dto_s;
using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Infrastructure.Identity;
using Dokterspraktijk.WebUI.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dokterspraktijk.WebUI.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly IDokterRepository _dokterRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAfspraakService _afspraakService;
        private readonly UserManager<ApplicationUser> _userManager;


        public AdminController(
            IDokterRepository dokterRepository,
            IPatientRepository patientRepository,
            IAfspraakService afspraakService,
            UserManager<ApplicationUser> userManager)
        {
            _dokterRepository = dokterRepository;
            _patientRepository = patientRepository;
            _afspraakService = afspraakService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? dokterId, DateOnly? datum, string? zoektermPatient)
        {
            DateOnly gekozenDatum = datum ?? DateOnly.FromDateTime(DateTime.Today);
            DateOnly vandaag = DateOnly.FromDateTime(DateTime.Today);
            DateOnly eindeWeek = vandaag.AddDays(7);

            ApplicationUser? admin = await _userManager.GetUserAsync(User);

            string adminNaam = "admin";

            if (admin != null)
            {
                adminNaam = admin.Voornaam + " " + admin.Achternaam;
            }

            List<Dokter> dokters = _dokterRepository.GeefAlleDokters();
            List<Patient> patienten = _patientRepository.GeefAllePatienten();

            if (!string.IsNullOrWhiteSpace(zoektermPatient))
            {
                string zoektermLower = zoektermPatient.ToLower();

                patienten = patienten
                    .Where(patient =>
                        patient.Voornaam.ToLower().Contains(zoektermLower) ||
                        patient.Achternaam.ToLower().Contains(zoektermLower) ||
                        patient.Email.ToLower().Contains(zoektermLower) ||
                        patient.Telefoonnummer.ToLower().Contains(zoektermLower))
                    .ToList();
            }

            List<AfspraakDto> alleAfspraken = _afspraakService.GeefAlleAfspraken();

            List<AfspraakDto> gefilterdeAfspraken = alleAfspraken
                .Where(afspraak => afspraak.Datum == gekozenDatum)
                .OrderBy(afspraak => afspraak.Tijd)
                .ToList();

            if (dokterId.HasValue)
            {
                Dokter? gekozenDokter = _dokterRepository.ZoekOpId(dokterId.Value);

                if (gekozenDokter != null)
                {
                    gefilterdeAfspraken = gefilterdeAfspraken
                        .Where(afspraak => afspraak.DokterNaam == gekozenDokter.Naam)
                        .ToList();
                }
            }

            List<PatientAccountViewModel> patientAccountViewModels = new List<PatientAccountViewModel>();

            foreach (Patient patient in patienten)
            {
                ApplicationUser? user = await _userManager.FindByEmailAsync(patient.Email);

                bool isAccountActief = false;

                if (user != null)
                {
                    DateTimeOffset? lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                    isAccountActief = lockoutEnd == null || lockoutEnd <= DateTimeOffset.UtcNow;
                }

                PatientAccountViewModel patientViewModel = new PatientAccountViewModel
                {
                    Id = patient.Id,
                    Voornaam = patient.Voornaam,
                    Achternaam = patient.Achternaam,
                    Email = patient.Email,
                    Telefoonnummer = patient.Telefoonnummer,
                    Rijksregisternummer = patient.Rijksregisternummer,
                    IsAccountActief = isAccountActief
                };

                patientAccountViewModels.Add(patientViewModel);
            }

            List<DokterOverzichtViewModel> dokterViewModels = new List<DokterOverzichtViewModel>();

            foreach (Dokter dokter in dokters)
            {
                bool isActief = await IsDokterAccountActief(dokter.Naam);

                DokterOverzichtViewModel dokterViewModel = new DokterOverzichtViewModel
                {
                    Id = dokter.Id,
                    Naam = dokter.Naam,
                    Specialisatie = dokter.Specialisatie,
                    IsActief = isActief
                };

                dokterViewModels.Add(dokterViewModel);
            }

            AdminDashboardViewModel viewModel = new AdminDashboardViewModel
            {
                AdminNaam = adminNaam,

                GekozenDokterId = dokterId,
                GekozenDatum = gekozenDatum,
                ZoektermPatient = zoektermPatient,

                AantalDokters = dokters.Count,
                AantalPatienten = _patientRepository.GeefAllePatienten().Count,
                AantalAfsprakenVandaag = alleAfspraken.Count(afspraak => afspraak.Datum == vandaag),
                AantalAfsprakenDezeWeek = alleAfspraken.Count(afspraak =>
                    afspraak.Datum >= vandaag &&
                    afspraak.Datum <= eindeWeek),

                Dokters = dokterViewModels,

                AfsprakenVandaag = gefilterdeAfspraken,

                Patienten = patientAccountViewModels
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Afspraken(int? dokterId, DateOnly? datum)
        {
            DateOnly gekozenDatum = datum ?? DateOnly.FromDateTime(DateTime.Today);

            List<Dokter> dokters = _dokterRepository.GeefAlleDokters();

            List<AfspraakDto> afspraken = _afspraakService.GeefAlleAfspraken()
                .Where(afspraak => afspraak.Datum == gekozenDatum)
                .OrderBy(afspraak => afspraak.Tijd)
                .ToList();

            if (dokterId.HasValue)
            {
                Dokter? gekozenDokter = _dokterRepository.ZoekOpId(dokterId.Value);

                if (gekozenDokter != null)
                {
                    afspraken = afspraken
                        .Where(afspraak => afspraak.DokterNaam == gekozenDokter.Naam)
                        .ToList();
                }
            }

            AdminAfsprakenViewModel viewModel = new AdminAfsprakenViewModel
            {
                GekozenDokterId = dokterId,
                GekozenDatum = gekozenDatum,
                Dokters = dokters
                    .Select(dokter => new DokterOverzichtViewModel
                    {
                        Id = dokter.Id,
                        Naam = dokter.Naam,
                        Specialisatie = dokter.Specialisatie
                    })
                    .ToList(),
                Afspraken = afspraken
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Patienten(string? zoekterm)
        {
            List<Patient> patienten = _patientRepository.GeefAllePatienten();

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

            List<PatientAccountViewModel> patientViewModels = new List<PatientAccountViewModel>();

            foreach (Patient patient in patienten)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(patient.Email)
                    ?? throw new InvalidOperationException("Elke patiënt moet een gekoppeld account hebben.");

                DateTimeOffset? lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);

                bool isAccountActief = lockoutEnd == null || lockoutEnd <= DateTimeOffset.UtcNow;

                PatientAccountViewModel patientViewModel = new PatientAccountViewModel
                {
                    Id = patient.Id,
                    Voornaam = patient.Voornaam,
                    Achternaam = patient.Achternaam,
                    Email = patient.Email,
                    Telefoonnummer = patient.Telefoonnummer,
                    Rijksregisternummer = patient.Rijksregisternummer,
                    IsAccountActief = isAccountActief
                };

                patientViewModels.Add(patientViewModel);
            }

            AdminPatientenViewModel viewModel = new AdminPatientenViewModel
            {
                Zoekterm = zoekterm,
                Patienten = patientViewModels
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveerPatientAccount(int id)
        {
            Patient? patient = _patientRepository.ZoekOpId(id);

            if (patient == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ApplicationUser user = await _userManager.FindByEmailAsync(patient.Email)
                ?? throw new InvalidOperationException("Elke patiënt moet een gekoppeld account hebben.");

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActiveerPatientAccount(int id)
        {
            Patient? patient = _patientRepository.ZoekOpId(id);

            if (patient == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ApplicationUser user = await _userManager.FindByEmailAsync(patient.Email)
                ?? throw new InvalidOperationException("Elke patiënt moet een gekoppeld account hebben.");

            await _userManager.SetLockoutEndDateAsync(user, null);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Dokters()
        {
            AdminDoktersViewModel viewModel = new AdminDoktersViewModel
            {
                Dokters = _dokterRepository.GeefAlleDokters()
                    .Select(dokter => new DokterOverzichtViewModel
                    {
                        Id = dokter.Id,
                        Naam = dokter.Naam,
                        Specialisatie = dokter.Specialisatie
                    })
                    .ToList()
            };

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult CreateDokter()
        {
            AdminDokterFormViewModel viewModel = new AdminDokterFormViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDokter(AdminDokterFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            Dokter? bestaandeDokter = _dokterRepository.ZoekOpNaam(viewModel.Naam);

            if (bestaandeDokter != null)
            {
                ModelState.AddModelError(nameof(viewModel.Naam), "Er bestaat al een dokter met deze naam.");
                return View(viewModel);
            }

            Dokter dokter = new Dokter
            {
                Naam = viewModel.Naam,
                Specialisatie = viewModel.Specialisatie
            };

            _dokterRepository.VoegToe(dokter);

            return RedirectToAction(nameof(Dokters));
        }
        [HttpGet]
        public IActionResult EditDokter(int id)
        {
            Dokter? dokter = _dokterRepository.ZoekOpId(id);

            if (dokter == null)
            {
                return RedirectToAction(nameof(Dokters));
            }

            AdminDokterFormViewModel viewModel = new AdminDokterFormViewModel
            {
                Id = dokter.Id,
                Naam = dokter.Naam,
                Specialisatie = dokter.Specialisatie
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDokter(AdminDokterFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            Dokter? dokter = _dokterRepository.ZoekOpId(viewModel.Id);

            if (dokter == null)
            {
                return RedirectToAction(nameof(Dokters));
            }

            dokter.Naam = viewModel.Naam;
            dokter.Specialisatie = viewModel.Specialisatie;

            _dokterRepository.WerkBij(dokter);

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult AfspraakDetails(int id)
        {
            AfspraakDto? afspraak = _afspraakService.ZoekAfspraakOpId(id);

            if (afspraak == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(afspraak);
        }
        [HttpGet]
        public IActionResult DokterPlanning(int dokterId, DateOnly? datum)
        {
            Dokter? dokter = _dokterRepository.ZoekOpId(dokterId);

            if (dokter == null)
            {
                return RedirectToAction(nameof(Index));
            }

            DateOnly gekozenDatum = datum ?? DateOnly.FromDateTime(DateTime.Today);

            List<AfspraakDto> afspraken = _afspraakService.GeefAlleAfspraken()
                .Where(afspraak =>
                    afspraak.DokterNaam == dokter.Naam &&
                    afspraak.Datum == gekozenDatum)
                .OrderBy(afspraak => afspraak.Tijd)
                .ToList();

            AdminDokterPlanningViewModel viewModel = new AdminDokterPlanningViewModel
            {
                DokterId = dokter.Id,
                DokterNaam = dokter.Naam,
                GekozenDatum = gekozenDatum,
                Afspraken = afspraken,
                AantalGepland = afspraken.Count(afspraak => afspraak.Status == "Gepland"),
                AantalAfgerond = afspraken.Count(afspraak => afspraak.Status == "Afgerond"),
                AantalGeannuleerd = afspraken.Count(afspraak => afspraak.Status == "Geannuleerd")
            };

            return View(viewModel);
        }
        private async Task<ApplicationUser?> ZoekDokterAccountOpNaam(string dokterNaam)
        {
            IList<ApplicationUser> gebruikers = await _userManager.GetUsersForClaimAsync(
                new Claim("DokterNaam", dokterNaam));

            return gebruikers.FirstOrDefault();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveerDokter(int id)
        {
            Dokter? dokter = _dokterRepository.ZoekOpId(id);

            if (dokter == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ApplicationUser? dokterUser = await ZoekDokterAccountOpNaam(dokter.Naam);

            if (dokterUser == null)
            {
                throw new InvalidOperationException("Deze dokter heeft geen gekoppeld account.");
            }

            await _userManager.SetLockoutEnabledAsync(dokterUser, true);
            await _userManager.SetLockoutEndDateAsync(dokterUser, DateTimeOffset.UtcNow.AddYears(100));

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActiveerDokter(int id)
        {
            Dokter? dokter = _dokterRepository.ZoekOpId(id);

            if (dokter == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ApplicationUser? dokterUser = await ZoekDokterAccountOpNaam(dokter.Naam);

            if (dokterUser == null)
            {
                throw new InvalidOperationException("Deze dokter heeft geen gekoppeld account.");
            }

            await _userManager.SetLockoutEndDateAsync(dokterUser, null);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> IsDokterAccountActief(string dokterNaam)
        {
            ApplicationUser? dokterUser = await ZoekDokterAccountOpNaam(dokterNaam);

            if (dokterUser == null)
            {
                return false;
            }

            DateTimeOffset? lockoutEnd = await _userManager.GetLockoutEndDateAsync(dokterUser);

            return lockoutEnd == null || lockoutEnd <= DateTimeOffset.UtcNow;
        }
    }
}