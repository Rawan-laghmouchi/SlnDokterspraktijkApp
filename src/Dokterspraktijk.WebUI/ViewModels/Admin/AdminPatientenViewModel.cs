namespace Dokterspraktijk.WebUI.ViewModels.Admin
{
    public class AdminPatientenViewModel
    {
        public string? Zoekterm { get; set; }
        public List<PatientAccountViewModel> Patienten { get; set; } = new List<PatientAccountViewModel>();
    }
}
