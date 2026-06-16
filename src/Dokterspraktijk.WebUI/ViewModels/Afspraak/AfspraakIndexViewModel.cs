using Dokterspraktijk.Application.Dto_s;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dokterspraktijk.WebUI.ViewModels.Afspraak
{
    public class AfspraakIndexViewModel
    {
        public List<AfspraakDto> Afspraken { get; set; } = new List<AfspraakDto>();

        public string? GekozenStatus { get; set; }

        public List<SelectListItem> StatusOpties { get; set; } = new List<SelectListItem>();

        public Dictionary<int, DoktersattestDto?> DoktersattestenPerAfspraakId { get; set; } = new Dictionary<int, DoktersattestDto?>();
    }
}