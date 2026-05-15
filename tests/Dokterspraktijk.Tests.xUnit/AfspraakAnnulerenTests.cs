using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Tests.xUnit
{
    public class AfspraakAnnulerenTests
    {
        [Fact]
        public void AfspraakAnnuleren_AfspraakLigtInDeToekomst_AnnuleertAfspraak()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public void AfspraakAnnuleren_AfspraakIsAlAfgerond_ThrowsInvalidOperationException()
        {
            // Arrange

            // Act & Assert
        }

        [Fact]
        public void AfspraakAnnuleren_AfspraakBehoortNietTotPatient_ThrowsUnauthorizedAccessException()
        {
            // Arrange

            // Act & Assert
        }
    }
}
