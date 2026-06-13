namespace Dokterspraktijk.WebUI.ViewModels.Admin
{
    public class PatientAccountViewModel
    {
        public int Id { get; set; }

        public string Voornaam { get; set; } = string.Empty;

        public string Achternaam { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefoonnummer { get; set; } = string.Empty;

        public string Rijksregisternummer { get; set; } = string.Empty;

        public bool IsAccountActief { get; set; }

        public string AccountStatus
        {
            get
            {
                if (IsAccountActief)
                {
                    return "Actief account";
                }

                return "Gedeactiveerd";
            }
        }

        public string VolledigeNaam
        {
            get
            {
                return Voornaam + " " + Achternaam;
            }
        }
    }
}