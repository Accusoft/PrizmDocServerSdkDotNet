using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Accusoft.PrizmDocServer.Burning.UnknownServerErrors.Tests
{
    [TestClass]
    public class UnknownGetError_Tests
    {
        private static PrizmDocServerClient prizmDocServer;
        private static FluentMockServer mockServer;

        [ClassInitialize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required MSTest Signature")]
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
              .Given(Request.Create().WithPath("/PCCIS/V1/MarkupBurner").UsingPost())
              .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"processId\":\"fake-process-id\",\"expirationDateTime\":\"2020-01-06T16:50:45.637Z\",\"input\":{\"documentFileId\":\"fake-file-id\",\"markupFileId\":\"fake-file-id\"},\"state\":\"processing\",\"percentComplete\":0}"));
        }

        [TestMethod]
        public async Task Unexpected_200_with_errorCode_on_GET()
        {
            mockServer
              .Given(Request.Create().WithPath("/PCCIS/V1/MarkupBurner/fake-process-id").UsingGet())
              .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"processId\":\"fake-process-id\",\"expirationDateTime\":\"2020-01-06T16:50:45.637Z\",\"input\":{\"documentFileId\":\"fake-file-id\",\"markupFileId\":\"fake-file-id\"},\"state\":\"error\",\"percentComplete\":100,\"errorCode\":\"ServerOnFire\",\"errorDetails\":{\"temperature\":999}}"));

            string expectedMessage = @"Remote server returned an error: ServerOnFire {
  ""temperature"": 999
}";

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () => { await prizmDocServer.BurnMarkupAsync("documents/confidential-contacts.pdf", "documents/confidential-contacts.pdf.markup.json"); },
                expectedMessage);
        }
    }
}
