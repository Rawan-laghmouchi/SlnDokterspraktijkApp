using Dokterspraktijk.Tests.BDD.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dokterspraktijk.Tests.BDD.StepDefinitions
{
    [Binding]
    public class GemeenschappelijkeResultaatStepDefinitions
    {
        private DokterspraktijkScenarioContext _context;

        public GemeenschappelijkeResultaatStepDefinitions(DokterspraktijkScenarioContext context)
        {
            _context = context;
        }

        [Then(@"wordt deze actie geweigerd")]
        public void ThenWordtDezeActieGeweigerd()
        {
            Assert.NotNull(_context.LaatsteResultaat);
            Assert.False(_context.LaatsteResultaat.IsGelukt);
        }
    }
}
