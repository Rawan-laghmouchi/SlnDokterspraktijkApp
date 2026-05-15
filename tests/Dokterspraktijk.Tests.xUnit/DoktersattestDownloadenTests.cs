using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Tests.xUnit
{
    public class DoktersattestDownloadenTests
    {
        [Fact]
        public void DoktersattestDownloaden_AttestIsVrijgegeven_GeeftAttestTerug()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public void DoktersattestDownloaden_AttestIsNietVrijgegeven_ThrowsInvalidOperationException()
        {
            // Arrange

            // Act & Assert
        }

        [Fact]
        public void DoktersattestDownloaden_AttestBehoortNietTotPatient_ThrowsUnauthorizedAccessException()
        {
            // Arrange

            // Act & Assert
        }
    }
}
