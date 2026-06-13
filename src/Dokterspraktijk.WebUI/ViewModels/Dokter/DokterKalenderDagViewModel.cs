namespace Dokterspraktijk.WebUI.ViewModels.Dokter
{
    public class DokterKalenderDagViewModel
    {
        public DateOnly Datum { get; set; }

        public bool IsBinnenMaand { get; set; }

        public bool IsVandaag { get; set; }

        public bool IsGeselecteerd { get; set; }

        public bool HeeftAfspraken { get; set; }

        public int AantalAfspraken { get; set; }

        public string DatumRouteWaarde { get
            {
                return Datum.ToString("yyyy-MM-dd");
            }
        }
    }
}