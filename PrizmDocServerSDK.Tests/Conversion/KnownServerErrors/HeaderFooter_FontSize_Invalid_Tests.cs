using System.Collections.Generic;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class HeaderFooter_FontSize_Invalid_Tests
    {
        [TestMethod]
        public async Task Invalid_header_font_size()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Header = new HeaderFooterOptions
                        {
                            FontSize = "wat",
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Test" },
                            },
                        },
                    });
                }, "Invalid Header.FontSize value for remote server: \"wat\"");
        }

        [TestMethod]
        public async Task Invalid_footer_font_size()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Footer = new HeaderFooterOptions
                        {
                            FontSize = "waaat",
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Test" },
                            },
                        },
                    });
                }, "Invalid Footer.FontSize value for remote server: \"waaat\"");
        }
    }
}
