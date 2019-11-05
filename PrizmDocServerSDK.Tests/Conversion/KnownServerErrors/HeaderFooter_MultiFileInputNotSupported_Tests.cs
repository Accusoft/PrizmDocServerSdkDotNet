using System.Collections.Generic;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class HeaderFooter_MultiFileInputNotSupported_Tests
    {
        [TestMethod]
        public async Task When_applying_a_header_to_multiple_sources()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(
                        new List<SourceDocument>
                        {
                            new SourceDocument("documents/example.pdf"),
                            new SourceDocument("documents/example.pdf"),
                            new SourceDocument("documents/example.pdf"),
                        },
                        new DestinationOptions(DestinationFileFormat.Pdf)
                        {
                            Header = new HeaderFooterOptions()
                            {
                                Lines = new List<HeaderFooterLine>()
                                {
                                    new HeaderFooterLine() { Left = "Acme" },
                                },
                            },
                        });
                }, "Remote server does not support applying headers or footers when using multiple SourceDocument instances. To apply headers or footers, use a single SourceDocument instance.");
        }

        [TestMethod]
        public async Task When_applying_a_footer_to_multiple_sources()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(
                        new List<SourceDocument>
                        {
                            new SourceDocument("documents/example.pdf"),
                            new SourceDocument("documents/example.pdf"),
                            new SourceDocument("documents/example.pdf"),
                        },
                        new DestinationOptions(DestinationFileFormat.Pdf)
                        {
                            Footer = new HeaderFooterOptions()
                            {
                                Lines = new List<HeaderFooterLine>()
                                {
                                    new HeaderFooterLine() { Left = "Acme" },
                                },
                            },
                        });
                }, "Remote server does not support applying headers or footers when using multiple SourceDocument instances. To apply headers or footers, use a single SourceDocument instance.");
        }
    }
}
