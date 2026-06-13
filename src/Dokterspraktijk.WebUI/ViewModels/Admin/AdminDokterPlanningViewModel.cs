using Dokterspraktijk.Application.Dto_s;

namespace Dokterspraktijk.WebUI.ViewModels.Admin
{
    public class AdminDokterPlanningViewModel
    {
        public int DokterId { get; set; }

        public string DokterNaam { get; set; } = string.Empty;

        public DateOnly GekozenDatum { get; set; }

        public List<AfspraakDto> Afspraken { get; set; } = new List<AfspraakDto>();

        public int AantalGepland { get; set; }

        public int AantalAfgerond { get; set; }

        public int AantalGeannuleerd { get; set; }
    }
}
