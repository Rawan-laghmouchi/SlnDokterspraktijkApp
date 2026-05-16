using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Domain.Entities
{
    public class Doktersattest
    {
        public int Id { get; private set; }
        public int AfspraakId { get; private set; }
        public bool IsVrijgegeven { get; private set; }
        public bool IsGedownload { get; private set; }

        public Doktersattest(int id, int afspraakId)
        {
            Id = id;
            AfspraakId = afspraakId;
            IsVrijgegeven = false;
            IsGedownload = false;
        }

        public void GeefVrij()
        {
            IsVrijgegeven = true;
        }
        public bool KanGedownloadWorden()
        {
            return IsVrijgegeven;
        }

        public void Download()
        {
            IsGedownload = true;
        }
    }
}
