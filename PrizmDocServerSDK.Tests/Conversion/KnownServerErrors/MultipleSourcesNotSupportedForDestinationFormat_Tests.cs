using System.Collections.Generic;
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
    public class MultipleSourcesNotSupportedForDestinationFormat_Tests
    {
        private static PrizmDocServerClient prizmDocServer;
        private static FluentMockServer mockServer;
        private SourceDocument dummySource = new SourceDocument(new RemoteWorkFile(null, null, null, null));

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
              .Given(Request.Create().WithPath("/v2/contentConverters").UsingPost())
              .RespondWith(Response.Create()
                .WithStatusCode(480)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"errorCode\":\"MultipleSourcesAreNotSupportedForThisDestinationFormat\",\"errorDetails\":{\"in\":\"body\",\"at\":\"input.sources\"}}"));
        }

        [TestMethod]
        public async Task Multiple_to_SVG()
        {
            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(
                        new List<SourceDocument>
                        {
                            new SourceDocument("documents/example.docx"),
                            new SourceDocument("documents/example.pdf"),
                        },
                        new DestinationOptions(DestinationFileFormat.Svg));
                }, "Remote server does not support combining multiple SourceDocument instances to SVG. When converting to SVG, use a single SourceDocument.");
        }

        [TestMethod]
        public async Task Multiple_to_PNG()
        {
            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(
                        new List<SourceDocument>
                        {
                            new SourceDocument("documents/example.docx"),
                            new SourceDocument("documents/example.pdf"),
                        },
                        new DestinationOptions(DestinationFileFormat.Png));
                }, "Remote server does not support combining multiple SourceDocument instances to PNG. When converting to PNG, use a single SourceDocument.");
        }

        [TestMethod]
        public async Task Multiple_to_JPEG()
        {
            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(
                        new List<SourceDocument>
                        {
                            new SourceDocument("documents/example.docx"),
                            new SourceDocument("documents/example.pdf"),
                        },
                        new DestinationOptions(DestinationFileFormat.Jpeg));
                }, "Remote server does not support combining multiple SourceDocument instances to JPEG. When converting to JPEG, use a single SourceDocument.");
        }

        [TestMethod]
        public async Task Multiple_to_DOCX()
        {
            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(
                        new List<SourceDocument>
                        {
                            new SourceDocument("documents/example.docx"),
                            new SourceDocument("documents/example.pdf"),
                        },
                        new DestinationOptions(DestinationFileFormat.Docx));
                }, "Remote server does not support combining multiple SourceDocument instances to DOCX. When converting to DOCX, use a single SourceDocument.");
        }
    }
}
