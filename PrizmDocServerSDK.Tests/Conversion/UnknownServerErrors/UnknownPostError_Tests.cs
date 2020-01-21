using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Accusoft.PrizmDocServer.Conversion.UnknownServerErrors.Tests
{
    [TestClass]
    public class UnknownPostError_Tests
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
        public async Task Unknown_error_on_POST()
        {
            mockServer
              .Given(Request.Create().WithPath("/v2/contentConverters").UsingPost())
              .RespondWith(Response.Create()
                .WithStatusCode(418)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"errorCode\":\"BoilingOver\",\"errorDetails\":{\"in\":\"body\",\"at\":\"input.admin.enableTurboMode\"}}"));

            var dummyInput = new ConversionSourceDocument(new RemoteWorkFile(null, null, null, null));

            string expectedMessage = @"Remote server returned an error: I'm a teapot BoilingOver {
  ""in"": ""body"",
  ""at"": ""input.admin.enableTurboMode""
}";

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () => { await prizmDocServer.ConvertAsync(dummyInput, new DestinationOptions(DestinationFileFormat.Pdf)); },
                expectedMessage,
                ignoreCase: true);
        }
    }
}
