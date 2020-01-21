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

            mockServer
                .Given(Request.Create().WithPath("/PCCIS/V1/WorkFile").UsingPost())
                .RespondWith(Response.Create()
                    .WithSuccess()
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{\"fileId\":\"fake-file-id\"}"));
        }

        [TestMethod]
        public async Task Unexpected_480_with_errorCode_on_POST()
        {
            mockServer
              .Given(Request.Create().WithPath("/PCCIS/V1/MarkupBurner").UsingPost())
              .RespondWith(Response.Create()
                .WithStatusCode(480)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"errorCode\":\"ServerOnFire\",\"errorDetails\":{\"in\":\"body\",\"at\":\"input.admin.enableTurboMode\"}}"));

            string expectedMessage = @"Remote server returned an error: ServerOnFire {
  ""in"": ""body"",
  ""at"": ""input.admin.enableTurboMode""
}";

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () => { await prizmDocServer.BurnMarkupAsync("documents/confidential-contacts.pdf", "documents/confidential-contacts.pdf.markup.json"); },
                expectedMessage);
        }

        [TestMethod]
        public async Task Unexpected_bare_418_on_POST()
        {
            mockServer
              .Given(Request.Create().WithPath("/PCCIS/V1/MarkupBurner").UsingPost())
              .RespondWith(Response.Create().WithStatusCode(418));

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () => { await prizmDocServer.BurnMarkupAsync("documents/confidential-contacts.pdf", "documents/confidential-contacts.pdf.markup.json"); },
                expectedMessage: @"Remote server returned an error: I'm a teapot",
                ignoreCase: true);
        }
    }
}
