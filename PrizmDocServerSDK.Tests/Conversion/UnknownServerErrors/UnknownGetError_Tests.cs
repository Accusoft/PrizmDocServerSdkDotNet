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
    public class UnknownGetError_Tests
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

            mockServer
                .Given(Request.Create().WithPath("/PCCIS/V1/WorkFile").UsingPost())
                .RespondWith(Response.Create()
                    .WithSuccess()
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{\"fileId\":\"fake-file-id\"}"));

            mockServer
              .Given(Request.Create().WithPath("/v2/contentConverters").UsingPost())
              .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"processId\":\"fake-process-id\",\"expirationDateTime\":\"2020-01-06T16:50:45.637Z\",\"state\":\"processing\",\"percentComplete\":0}"));
        }

        [TestMethod]
        public async Task Final_status_is_something_other_than_complete_there_there_is_no_errorCode()
        {
            mockServer
              .Given(Request.Create().WithPath("/v2/contentConverters/fake-process-id").UsingGet())
              .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"processId\":\"fake-process-id\",\"expirationDateTime\":\"2020-01-06T16:50:45.637Z\",\"state\":\"dead\",\"percentComplete\":100}"));

            string expectedMessage = @"Unexpected conversion state ""dead"":
{""processId"":""fake-process-id"",""expirationDateTime"":""2020-01-06T16:50:45.637Z"",""state"":""dead"",""percentComplete"":100}";

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () => { await prizmDocServer.ConvertAsync("documents/example.pdf", DestinationFileFormat.Pdf); },
                expectedMessage);
        }
    }
}
