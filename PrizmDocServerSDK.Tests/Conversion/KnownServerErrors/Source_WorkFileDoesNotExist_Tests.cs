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
    public class Source_WorkFileDoesNotExist_Tests
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
        public async Task When_single_input_work_file_does_not_exist()
        {
            mockServer
              .Given(Request.Create().WithPath("/v2/contentConverters").UsingPost())
              .RespondWith(Response.Create()
                .WithStatusCode(480)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"input\":{\"dest\":{\"format\":\"pdf\",\"pdfOptions\":{\"forceOneFilePerPage\":false}},\"sources\":[{\"fileId\":\"ML3AbF-qzIH5K9mVVxTlBX\",\"pages\":\"\"}]},\"minSecondsAvailable\":18000,\"errorCode\":\"WorkFileDoesNotExist\",\"errorDetails\":{\"in\":\"body\",\"at\":\"input.sources[0].fileId\"}}"));

            var originalRemoteWorkFile = new RemoteWorkFile(null, "ML3AbF-qzIH5K9mVVxTlBX", "FCnaLL517YPRAnrcX2wlnKURpNPsp2d2pMPkcvCcpdY=", "docx");
            var originalConversionInput = new SourceDocument(originalRemoteWorkFile);

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
            {
                await prizmDocServer.ConvertAsync(originalConversionInput, new DestinationOptions(DestinationFileFormat.Pdf));
            }, "SourceDocument refers to a remote work file which does not exist. It may have expired.");
        }

        [TestMethod]
        public async Task When_the_second_of_three_input_work_files_does_not_exist()
        {
            var remoteWorkFile0 = new RemoteWorkFile(null, "ML3AbF-qzIH5K9mVVxTlBX", "FCnaLL517YPRAnrcX2wlnKURpNPsp2d2pMPkcvCcpdY=", "docx");
            var remoteWorkFile1 = new RemoteWorkFile(null, "S5uCdv7vnkTRzKKlTvhtaw", "FCnaLL517YPRAnrcX2wlnKURpNPsp2d2pMPkcvCcpdY=", "docx");
            var remoteWorkFile2 = new RemoteWorkFile(null, "5J15gtlduA_xORR8j7ejSg", "FCnaLL517YPRAnrcX2wlnKURpNPsp2d2pMPkcvCcpdY=", "docx");

            var input0 = new SourceDocument(remoteWorkFile0);
            var input1 = new SourceDocument(remoteWorkFile1, pages: "2-");
            var input2 = new SourceDocument(remoteWorkFile2);

            mockServer
              .Given(Request.Create().WithPath("/v2/contentConverters").UsingPost())
              .RespondWith(Response.Create()
                .WithStatusCode(480)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"input\":{\"dest\":{\"format\":\"pdf\",\"pdfOptions\":{\"forceOneFilePerPage\":false}},\"sources\":[{\"fileId\":\"LxuuLktmmMaicAs1wMvvsQ\",\"pages\":\"\"},{\"fileId\":\"S5uCdv7vnkTRzKKlTvhtaw\",\"pages\":\"2-\"},{\"fileId\":\"5J15gtlduA_xORR8j7ejSg\",\"pages\":\"\"}]},\"minSecondsAvailable\":18000,\"errorCode\":\"WorkFileDoesNotExist\",\"errorDetails\":{\"in\":\"body\",\"at\":\"input.sources[1].fileId\"}}"));

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
            {
                await prizmDocServer.ConvertAsync(
                    new List<SourceDocument>
                    {
                        input0, input1, input2,
                    },
                    new DestinationOptions(DestinationFileFormat.Pdf));
            }, "SourceDocument at index 1 refers to a remote work file which does not exist. It may have expired.");
        }
    }
}
