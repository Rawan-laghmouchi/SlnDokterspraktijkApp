using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Tests.xUnit
{
    public class AfspraakMakenTests
    {
        [Fact]
        public void AfspraakMaken_TijdslotIsBeschikbaar_RegistreertAfspraak()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public void AfspraakMaken_TijdslotIsNietBeschikbaar_ThrowsInvalidOperationException()
        {
            // Arrange

            // Act & Assert
        }

        [Fact]
        public void AfspraakMaken_GevraagdeDatumLigtInHetVerleden_ThrowsArgumentException()
        {
            // Arrange

            // Act & Assert
        }
    }
}
