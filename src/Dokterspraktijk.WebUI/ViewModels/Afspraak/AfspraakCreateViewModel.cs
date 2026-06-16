using Dokterspraktijk.WebUI.ViewModels.Dokter;
using Dokterspraktijk.WebUI.ViewModels.Tijdslot;
using System.ComponentModel.DataAnnotations;

namespace Dokterspraktijk.WebUI.ViewModels.Afspraak
{
    public class AfspraakCreateViewModel
    {
        [Required(ErrorMessage = "Voornaam is verplicht.")]
        [Display(Name = "Voornaam")]
        public string PatientVoornaam { get; set; } = string.Empty;

        [Required(ErrorMessage = "Achternaam is verplicht.")]
        [Display(Name = "Achternaam")]
        public string PatientAchternaam { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail is verplicht.")]
        [EmailAddress(ErrorMessage = "Geef een geldig e-mailadres in.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefoonnummer is verplicht.")]
        [Phone(ErrorMessage = "Geef een geldig telefoonnummer in.")]
        [Display(Name = "Telefoonnummer")]
        public string Telefoonnummer { get; set; } = string.Empty;

        [Display(Name = "Rijksregisternummer")]
        public string Rijksregisternummer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Naam van de dokter is verplicht.")]
        [Display(Name = "Dokter")]
        public string DokterNaam { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum is verplicht.")]
        [Display(Name = "Datum")]
        public DateOnly Datum { get; set; }

        [Required(ErrorMessage = "Tijdstip is verplicht.")]
        [Display(Name = "Tijdstip")]
        public TimeOnly Tijd { get; set; }

        [Required(ErrorMessage = "Afspraakcategorie kiezen is verplicht.")]
        [Display(Name = "Afspraakcategorie")]
        public string Reden { get; set; } = string.Empty;

        public string MinimumDatum { get; set; } = string.Empty;

        public List<DokterKeuzeViewModel> Dokters { get; set; } = new List<DokterKeuzeViewModel>();

        public List<TijdslotKeuzeViewModel> Tijdsloten { get; set; } = new List<TijdslotKeuzeViewModel>();

        public List<AfspraakCategorieViewModel> AfspraakCategorieen { get; set; } = new List<AfspraakCategorieViewModel>();

        public bool PatientGegevensZijnVoorafIngevuld { get; set; }
    }
}