using System.ComponentModel.DataAnnotations;

namespace Dokterspraktijk.WebUI.ViewModels
{
    public class AfspraakCreateViewModel
    {
        [Required(ErrorMessage = "Voornaam is verplicht.")]
        [Display(Name = "Voornaam")]
        public string PatientVoornaam { get; set; }

        [Required(ErrorMessage = "Achternaam is verplicht.")]
        [Display(Name = "Achternaam")]
        public string PatientAchternaam { get; set; }

        [Required(ErrorMessage = "E-mail is verplicht.")]
        [EmailAddress(ErrorMessage = "Geef een geldig e-mailadres in.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefoonnummer is verplicht.")]
        [Phone(ErrorMessage = "Geef een geldig telefoonnummer in.")]
        [Display(Name = "Telefoonnummer")]
        public string Telefoonnummer { get; set; }

        public string Rijksregisternummer { get; set; }

        [Required(ErrorMessage = "Geboortedatum is verplicht.")]
        [Display(Name = "Geboortedatum")]
        public DateOnly Geboortedatum { get; set; }

        [Required(ErrorMessage = "Naam van de dokter is verplicht.")]
        [Display(Name = "Dokter")]
        public string DokterNaam { get; set; }

        [Required(ErrorMessage = "Datum is verplicht.")]
        [Display(Name = "Datum")]
        public DateOnly Datum { get; set; }

        [Required(ErrorMessage = "Tijdstip is verplicht.")]
        [Display(Name = "Tijdstip")]
        public TimeOnly Tijd { get; set; }

        [Required(ErrorMessage = "Beschrijving is verplicht.")]
        [Display(Name = "Beschrijving")]
        public string Reden { get; set; }

        public List<DokterKeuzeViewModel> Dokters { get; set; } = new List<DokterKeuzeViewModel>();
        public List<TijdslotKeuzeViewModel> Tijdsloten { get; set; } = new List<TijdslotKeuzeViewModel>();
        public List<AfspraakCategorieViewModel> AfspraakCategorieen { get; set; } = new List<AfspraakCategorieViewModel>();
    }
}
