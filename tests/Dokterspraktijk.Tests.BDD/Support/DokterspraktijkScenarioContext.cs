using Dokterspraktijk.Application.Dto_s;

namespace Dokterspraktijk.Tests.BDD.Support
{
    public class DokterspraktijkScenarioContext
    {
        public ResultaatDto? LaatsteResultaat { get; set; }
        public AfspraakDto? LaatsteAfspraak { get; set; }
        public DoktersattestDto? LaatsteDoktersattest { get; set; }

        public List<TijdslotDto> GeraadpleegdeTijdsloten { get; set; }
        public List<AfspraakDto> GeraadpleegdeAfspraken { get; set; }

        public string? VoorgesteldeVoorkeursdokter { get; set; }

        public DokterspraktijkScenarioContext()
        {
            GeraadpleegdeTijdsloten = new List<TijdslotDto>();
            GeraadpleegdeAfspraken = new List<AfspraakDto>();
        }
    }
}