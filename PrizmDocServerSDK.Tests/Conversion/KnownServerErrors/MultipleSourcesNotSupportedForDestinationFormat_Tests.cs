using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accusoft.PrizmDocServer.Tests;
using Accusoft.PrizmDocServer.Exceptions;
using System.Collections.Generic;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using System.Linq;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
  [TestClass]
  public class MultipleSourcesNotSupportedForDestinationFormat_Tests
  {
    static PrizmDocServerClient prizmDocServer;
    static FluentMockServer mockServer;
    SourceDocument dummySource = new SourceDocument(new RemoteWorkFile(null, null, null, null));

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
      var context = prizmDocServer.CreateProcessingContext();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await context.ConvertAsync(new List<SourceDocument> {
          new SourceDocument("documents/example.docx"),
          new SourceDocument("documents/example.pdf")
        }, new DestinationOptions(DestinationFileFormat.Svg));
      }, "Remote server does not support combining multiple SourceDocument instances to SVG. When converting to SVG, use a single SourceDocument.");
    }

    [TestMethod]
    public async Task Multiple_to_PNG()
    {
      var context = prizmDocServer.CreateProcessingContext();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await context.ConvertAsync(new List<SourceDocument> {
          new SourceDocument("documents/example.docx"),
          new SourceDocument("documents/example.pdf")
        }, new DestinationOptions(DestinationFileFormat.Png));
      }, "Remote server does not support combining multiple SourceDocument instances to PNG. When converting to PNG, use a single SourceDocument.");
    }

    [TestMethod]
    public async Task Multiple_to_JPEG()
    {
      var context = prizmDocServer.CreateProcessingContext();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await context.ConvertAsync(new List<SourceDocument> {
          new SourceDocument("documents/example.docx"),
          new SourceDocument("documents/example.pdf")
        }, new DestinationOptions(DestinationFileFormat.Jpeg));
      }, "Remote server does not support combining multiple SourceDocument instances to JPEG. When converting to JPEG, use a single SourceDocument.");
    }

    [TestMethod]
    public async Task Multiple_to_DOCX()
    {
      var context = prizmDocServer.CreateProcessingContext();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await context.ConvertAsync(new List<SourceDocument> {
          new SourceDocument("documents/example.docx"),
          new SourceDocument("documents/example.pdf")
        }, new DestinationOptions(DestinationFileFormat.Docx));
      }, "Remote server does not support combining multiple SourceDocument instances to DOCX. When converting to DOCX, use a single SourceDocument.");
    }
  }
}
