namespace Dokterspraktijk.WebUI.ViewModels.Dokter
{
    public class DokterPatientenViewModel
    {
        public string? Zoekterm { get; set; }
        public List<PatientOverzichtViewModel> Patienten { get; set; } = new List<PatientOverzichtViewModel>();
    }
}
