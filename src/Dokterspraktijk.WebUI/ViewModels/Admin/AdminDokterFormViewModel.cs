using System.ComponentModel.DataAnnotations;

namespace Dokterspraktijk.WebUI.ViewModels.Admin
{
    public class AdminDokterFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naam is verplicht.")]
        [StringLength(100, ErrorMessage = "Naam mag maximum 100 tekens bevatten.")]
        public string Naam { get; set; } = string.Empty;

        [Required(ErrorMessage = "Specialisatie is verplicht.")]
        [StringLength(100, ErrorMessage = "Specialisatie mag maximum 100 tekens bevatten.")]
        public string Specialisatie { get; set; } = string.Empty;
    }
}
