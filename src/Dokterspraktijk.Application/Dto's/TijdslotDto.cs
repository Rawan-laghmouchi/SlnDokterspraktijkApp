namespace Dokterspraktijk.Application.Dto_s
{
    public class TijdslotDto
    {
        public int Id { get; set; }
        public string DokterNaam { get; set; } = string.Empty;
        public DateOnly Datum { get; set; }
        public TimeOnly Tijd { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}