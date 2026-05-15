using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Tests.xUnit
{
    public class FotoToevoegenAanAfspraakaanvraagTests
    {
        [Theory]
        [InlineData("jpg")]
        [InlineData("jpeg")]
        [InlineData("png")]
        public void FotoToevoegen_BestandsextensieWordtOndersteund_KoppeltFotoAanAfspraak(string extension)
        {
            // Arrange

            // Act

            // Assert
        }

        [Theory]
        [InlineData("pdf")]
        [InlineData("docx")]
        [InlineData("txt")]
        public void FotoToevoegen_BestandsextensieWordtNietOndersteund_ThrowsArgumentException(string extension)
        {
            // Arrange

            // Act & Assert
        }
    }
}
