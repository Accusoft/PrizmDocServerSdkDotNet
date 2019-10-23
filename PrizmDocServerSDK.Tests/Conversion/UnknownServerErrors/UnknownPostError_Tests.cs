using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;

namespace Accusoft.PrizmDocServer.Conversion.UnknownServerErrors.Tests
{
  [TestClass]
  public class UnknownPostError_Tests
  {
    static PrizmDocServerClient prizmDocServer;
    static FluentMockServer mockServer;

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
          .WithStatusCode(480)
          .WithHeader("Content-Type", "application/json")
          .WithBody("{\"errorCode\":\"ServerOnFire\",\"errorDetails\":{\"in\":\"body\",\"at\":\"input.admin.enableTurboMode\"}}"));

      var context = prizmDocServer.CreateProcessingContext();

      var dummyInput = new SourceDocument(new RemoteWorkFile(null, null, null, null));

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await context.ConvertAsync(dummyInput, new DestinationOptions(DestinationFileFormat.Pdf));
      }, @"Remote server returned an error: ServerOnFire {
  ""in"": ""body"",
  ""at"": ""input.admin.enableTurboMode""
}");
    }
  }
}
