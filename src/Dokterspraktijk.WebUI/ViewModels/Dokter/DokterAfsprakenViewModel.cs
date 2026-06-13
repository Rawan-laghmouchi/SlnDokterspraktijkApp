using Dokterspraktijk.Application.Dto_s;

namespace Dokterspraktijk.WebUI.ViewModels.Dokter
{
    public class DokterAfsprakenViewModel
    {
        public string? GekozenStatus { get; set; }
        public List<AfspraakDto> Afspraken { get; set; } = new List<AfspraakDto>();
    }
}
