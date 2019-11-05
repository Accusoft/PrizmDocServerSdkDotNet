using System.Collections.Generic;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class HeaderFooter_Color_Invalid_Tests
    {
        [TestMethod]
        public async Task Invalid_header_color()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Header = new HeaderFooterOptions
                        {
                            Color = "rrrrED!",
                            Lines = new List<HeaderFooterLine>
                            {
                                 new HeaderFooterLine { Left = "Test" },
                            },
                        },
                    });
                }, "Invalid Header.Color value for remote server: \"rrrrED!\"");
        }

        [TestMethod]
        public async Task Invalid_footer_color()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Footer = new HeaderFooterOptions
                        {
                            Color = "Bluuuuue",
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Test" },
                            },
                        },
                    });
                }, "Invalid Footer.Color value for remote server: \"Bluuuuue\"");
        }
    }
}
