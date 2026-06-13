using Dokterspraktijk.Application.Dto_s;

namespace Dokterspraktijk.WebUI.ViewModels.Admin
{
    public class AdminAfsprakenViewModel
    {
        public int? GekozenDokterId { get; set; }
        public DateOnly GekozenDatum { get; set; }
        public List<DokterOverzichtViewModel> Dokters { get; set; } = new List<DokterOverzichtViewModel>();
        public List<AfspraakDto> Afspraken { get; set; } = new List<AfspraakDto>();
    }
}
