using System.Collections.Generic;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class HeaderFooter_UnavailableFontFamily_Tests
    {
        [TestMethod]
        public async Task Unavailable_font_family_with_header()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Header = new HeaderFooterOptions
                        {
                            FontFamily = "Lexicon",
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Test" },
                            },
                        },
                    });
                }, "Header.FontFamily or Footer.FontFamily specifies a font which is not available on the remote server.");
        }

        [TestMethod]
        public async Task Unavailable_font_family_with_footer()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Footer = new HeaderFooterOptions
                        {
                            FontFamily = "Lexicon",
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Test" },
                            },
                        },
                    });
                }, "Header.FontFamily or Footer.FontFamily specifies a font which is not available on the remote server.");
        }
    }
}
