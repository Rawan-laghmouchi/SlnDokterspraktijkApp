using Dokterspraktijk.Application.Dto_s;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dokterspraktijk.WebUI.ViewModels
{
    public class AfspraakIndexViewModel
    {
        public List<AfspraakDto> Afspraken { get; set; }
        public List<string> Statussen { get; set; }
        public string GekozenStatus { get; set; }
        public List<SelectListItem> StatusOpties { get; set; } = new List<SelectListItem>();
    }
}
