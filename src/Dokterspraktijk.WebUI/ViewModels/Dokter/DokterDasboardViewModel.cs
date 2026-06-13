using Dokterspraktijk.Application.Dto_s;
using System.Globalization;

namespace Dokterspraktijk.WebUI.ViewModels.Dokter
{
    public class DokterDashboardViewModel
    {
        public string DokterNaam { get; set; } = string.Empty;

        public DateOnly Vandaag { get; set; }

        public DateOnly GeselecteerdeDatum { get; set; }

        public List<AfspraakDto> AfsprakenVandaag { get; set; } = new List<AfspraakDto>();

        public List<AfspraakDto> KomendeAfspraken { get; set; } = new List<AfspraakDto>();

        public List<AfspraakDto> AfsprakenOpGeselecteerdeDatum { get; set; } = new List<AfspraakDto>();

        public List<DokterKalenderDagViewModel> KalenderDagen { get; set; } = new List<DokterKalenderDagViewModel>();
        public int AantalAfsprakenVandaag { get
            {
                return AfsprakenVandaag.Count;
            }
        }

        public int AantalKomendeAfspraken { get
            {
                return KomendeAfspraken.Count;
            }
        }

        public int AantalPatientenVandaag { get
            {
                return AfsprakenVandaag
                    .Select(afspraak => afspraak.PatientVoornaam + " " + afspraak.PatientAchternaam)
                    .Distinct()
                    .Count();
            }
        }

        public string KalenderTitel { get
            {
                return GeselecteerdeDatum.ToString("MMMM yyyy", new CultureInfo("nl-BE"));
            }
        }
        public string GeselecteerdeDatumTekst { get
            {
                return GeselecteerdeDatum.ToString("dd/MM/yyyy");
            }
        }
    }
}