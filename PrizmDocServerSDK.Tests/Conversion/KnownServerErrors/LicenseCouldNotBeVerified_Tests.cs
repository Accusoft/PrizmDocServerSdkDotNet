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
    public class LicenseCouldNotBeVerified_Tests
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
        public async Task When_the_product_is_not_licensed()
        {
            mockServer
              .Given(Request.Create().WithPath("/v2/contentConverters").UsingPost())
              .RespondWith(Response.Create()
                .WithStatusCode(480)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"errorCode\":\"LicenseCouldNotBeVerified\"}"));

            var dummyInput = new ConversionSourceDocument(new RemoteWorkFile(null, null, null, null));

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
            {
                await prizmDocServer.ConvertAsync(dummyInput, new DestinationOptions(DestinationFileFormat.Pdf));
            }, "Remote server does not have a valid license.");
        }
    }
}
