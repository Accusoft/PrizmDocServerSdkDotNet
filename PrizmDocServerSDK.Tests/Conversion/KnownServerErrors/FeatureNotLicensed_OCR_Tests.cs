using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class FeatureNotLicensed_OCR_Tests
    {
        private static PrizmDocServerClient prizmDocServer;
        private static FluentMockServer mockServer;

        [ClassInitialize]
        public static void BeforeAll(TestContext context)
        {
            mockServer = FluentMockServer.Start();
            prizmDocServer = new PrizmDocServerClient("http://localhost:" + mockServer.Ports.First());
        }

        [ClassCleanup]
        public static void AfterAll()
        {
            mockServer.Stop();
            mockServer.Dispose();
        }

        [TestInitialize]
        public void BeforeEach()
        {
            mockServer.Reset();
        }

        [TestMethod]
        public async Task When_requesting_PDF_OCR_but_the_feature_is_not_licensed()
        {
            mockServer
              .Given(Request.Create().WithPath("/v2/contentConverters").UsingPost())
              .RespondWith(Response.Create()
                .WithStatusCode(480)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"errorCode\":\"FeatureNotLicensed\",\"errorDetails\":{\"in\":\"body\",\"at\":\"input.dest.pdfOptions.ocr\"}}"));

            var dummyInput = new SourceDocument(new RemoteWorkFile(null, null, null, null));

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(dummyInput, new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        PdfOptions = new PdfDestinationOptions
                        {
                            Ocr = new OcrOptions
                            {
                                Language = "english",
                            },
                        },
                    });
                }, "Remote server is not licensed to perform OCR when producing PDF output.");
        }
    }
}
