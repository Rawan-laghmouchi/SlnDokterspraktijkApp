namespace Dokterspraktijk.WebUI.ViewModels.Admin
{
    public class DokterOverzichtViewModel
    {
        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;
        public string Specialisatie { get; set; } = string.Empty;
        public bool IsActief { get; set; }
    }
}
