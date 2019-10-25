using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accusoft.PrizmDocServer.Tests;
using System.Linq;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
  [TestClass]
  [TestCategory("Slow")]
  public class OcrToPdfAsync_Tests
  {
    [TestMethod]
    public async Task Single_input()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();
      var result = await prizmDocServer.OcrToPdfAsync("documents/ocr/chaucer-scan-3-pages.pdf");
      Assert.IsTrue(result.IsSuccess);
      Assert.AreEqual(3, result.PageCount);

      var resultSourceDocument = result.Sources.ToList()[0];
      Assert.IsNotNull(resultSourceDocument.RemoteWorkFile);
      Assert.IsNull(resultSourceDocument.Password);
      Assert.AreEqual("1-3", resultSourceDocument.Pages);

      await result.RemoteWorkFile.SaveAsync("output.pdf");
      FileAssert.IsPdf("output.pdf");
    }

    [TestMethod]
    public async Task Just_the_first_page()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();
      var sourceDocument = new SourceDocument("documents/ocr/chaucer-scan-3-pages.pdf", pages: "1");
      var result = await prizmDocServer.ConvertToPdfAsync(sourceDocument);
      Assert.IsTrue(result.IsSuccess);
      Assert.AreEqual(1, result.PageCount);

      var resultSourceDocument = result.Sources.ToList()[0];
      Assert.AreEqual(sourceDocument.RemoteWorkFile, resultSourceDocument.RemoteWorkFile);
      Assert.IsNull(resultSourceDocument.Password);
      Assert.AreEqual("1", resultSourceDocument.Pages);

      await result.RemoteWorkFile.SaveAsync("output.pdf");
      FileAssert.IsPdf("output.pdf");
    }

    [TestMethod]
    public async Task Multiple_inputs()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();
      var sourceDocument1 = new SourceDocument("documents/ocr/color.bmp");
      var sourceDocument2 = new SourceDocument("documents/ocr/text.bmp");
      var result = await prizmDocServer.OcrToPdfAsync(new SourceDocument[] { sourceDocument1, sourceDocument2 });
      Assert.IsTrue(result.IsSuccess);
      Assert.AreEqual(2, result.PageCount);

      var resultSourceDocuments = result.Sources.ToList();

      Assert.AreEqual(sourceDocument1.RemoteWorkFile, resultSourceDocuments[0].RemoteWorkFile);
      Assert.IsNull(resultSourceDocuments[0].Password);
      Assert.AreEqual("1", resultSourceDocuments[0].Pages);

      Assert.AreEqual(sourceDocument2.RemoteWorkFile, resultSourceDocuments[1].RemoteWorkFile);
      Assert.IsNull(resultSourceDocuments[1].Password);
      Assert.AreEqual("1", resultSourceDocuments[1].Pages);

      await result.RemoteWorkFile.SaveAsync("output.pdf");
      FileAssert.IsPdf("output.pdf");
    }
  }
}
