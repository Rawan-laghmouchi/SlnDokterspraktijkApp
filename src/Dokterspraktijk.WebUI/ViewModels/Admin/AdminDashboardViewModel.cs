using Dokterspraktijk.Application.Dto_s;
namespace Dokterspraktijk.WebUI.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public string AdminNaam { get; set; } = string.Empty;
        public int? GekozenDokterId { get; set; }
        public DateOnly GekozenDatum { get; set; }
        public string? ZoektermPatient { get; set; }
        public int AantalDokters { get; set; }
        public int AantalPatienten { get; set; }
        public int AantalAfsprakenVandaag { get; set; }
        public int AantalAfsprakenDezeWeek { get; set; }
        public List<DokterOverzichtViewModel> Dokters { get; set; } = new List<DokterOverzichtViewModel>();
        public List<AfspraakDto> AfsprakenVandaag { get; set; } = new List<AfspraakDto>();
        public List<PatientAccountViewModel> Patienten { get; set; } = new List<PatientAccountViewModel>();

    }
}
