namespace Dokterspraktijk.Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }

        public string Voornaam { get; set; }

        public string Achternaam { get; set; }

        public string Email { get; set; }

        public string Telefoonnummer { get; set; }

        public string Rijksregisternummer { get; set; }

        public int? VoorkeursdokterId { get; set; }

        public Patient()
        {
            Voornaam = string.Empty;
            Achternaam = string.Empty;
            Email = string.Empty;
            Telefoonnummer = string.Empty;
            Rijksregisternummer = string.Empty;
        }

        public Patient(int id, string voornaam, string achternaam)
        {
            Id = id;
            Voornaam = voornaam;
            Achternaam = achternaam;
            Email = string.Empty;
            Telefoonnummer = string.Empty;
            Rijksregisternummer = string.Empty;
        }

        public Patient(
            int id,
            string voornaam,
            string achternaam,
            string email,
            string telefoonnummer,
            string rijksregisternummer)
        {
            Id = id;
            Voornaam = voornaam;
            Achternaam = achternaam;
            Email = email;
            Telefoonnummer = telefoonnummer;
            Rijksregisternummer = rijksregisternummer;
        }
    }
}