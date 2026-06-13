using Dokterspraktijk.Application.Dto_s;

namespace Dokterspraktijk.WebUI.ViewModels.Dokter
{
    public class DokterAfspraakDetailsViewModel
    {
        public AfspraakDto Afspraak { get; set; } = new AfspraakDto();

        public DoktersattestDto? Doktersattest { get; set; }

        public bool MagDoktersattestVrijgeven
        {
            get
            {
                return Afspraak.Status == "Afgerond" &&
                       (Doktersattest == null || !Doktersattest.IsVrijgegeven);
            }
        }

        public bool IsDoktersattestVrijgegeven
        {
            get
            {
                return Doktersattest != null && Doktersattest.IsVrijgegeven;
            }
        }
    }
}