using System.Collections.Generic;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class Source_Pages_WithHeaderFooter_Tests
    {
        [TestMethod]
        public async Task When_applying_a_header_to_a_single_source_with_a_page_range_specified()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(new ConversionSourceDocument("documents/example.pdf", pages: "2-"), new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Header = new HeaderFooterOptions()
                        {
                            Lines = new List<HeaderFooterLine>()
                            {
                                new HeaderFooterLine() { Left = "Acme" },
                            },
                        },
                    });
                }, "Remote server does not support applying headers or footers when \"pages\" is specified for a ConversionSourceDocument.");
        }

        [TestMethod]
        public async Task When_applying_a_footer_to_a_single_source_with_a_page_range_specified()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(new ConversionSourceDocument("documents/example.pdf", pages: "2-"), new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Footer = new HeaderFooterOptions()
                        {
                            Lines = new List<HeaderFooterLine>()
                            {
                                new HeaderFooterLine() { Left = "Acme" },
                            },
                        },
                    });
                }, "Remote server does not support applying headers or footers when \"pages\" is specified for a ConversionSourceDocument.");
        }
    }
}
