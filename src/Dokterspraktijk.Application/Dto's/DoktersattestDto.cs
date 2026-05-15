using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Application.Dto_s
{
    public class DoktersattestDto
    {
        public int Id { get; set; }
        public int AfspraakId { get; set; }
        public bool IsVrijgegeven { get; set; }
        public bool IsGedownload { get; set; }
    }
}
