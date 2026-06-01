namespace Dokterspraktijk.Domain.Entities
{
    public class Doktersattest
    {
        public int Id { get; set; }
        public int AfspraakId { get; set; }
        public bool IsVrijgegeven { get; set; }
        public bool IsGedownload { get; set; }

        public Doktersattest()
        {
            IsVrijgegeven = false;
            IsGedownload = false;
        }

        public Doktersattest(int id, int afspraakId)
        {
            Id = id;
            AfspraakId = afspraakId;
            IsVrijgegeven = false;
            IsGedownload = false;
        }
    }
}