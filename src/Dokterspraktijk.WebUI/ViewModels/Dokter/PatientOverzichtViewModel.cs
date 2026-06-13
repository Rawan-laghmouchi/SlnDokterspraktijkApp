namespace Dokterspraktijk.WebUI.ViewModels.Dokter
{
    public class PatientOverzichtViewModel
    {
        public int Id { get; set; }

        public string Voornaam { get; set; } = string.Empty;

        public string Achternaam { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefoonnummer { get; set; } = string.Empty;

        public string Rijksregisternummer { get; set; } = string.Empty;

        public string VolledigeNaam { get
            {
                return Voornaam + " " + Achternaam;
            }
        }
    }
}

