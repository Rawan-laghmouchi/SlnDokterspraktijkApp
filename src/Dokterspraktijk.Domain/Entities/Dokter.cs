namespace Dokterspraktijk.Domain.Entities
{
    public class Dokter
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Specialisatie { get; set; }

        public Dokter()
        {
            Naam = string.Empty;
            Specialisatie = string.Empty;
        }

        public Dokter(int id, string naam, string specialisatie)
        {
            Id = id;
            Naam = naam;
            Specialisatie = specialisatie;
        }
    }
}