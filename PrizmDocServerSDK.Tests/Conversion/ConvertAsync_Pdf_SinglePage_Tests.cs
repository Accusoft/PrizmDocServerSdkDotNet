using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accusoft.PrizmDocServer.Tests;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
  [TestClass]
  public class ConvertAsync_Pdf_SinglePage_Tests
  {
    [TestMethod]
    public async Task With_local_file_path()
    {
      var context = Util.CreateContext();
      var results = await context.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
      {
        PdfOptions = new PdfDestinationOptions()
        {
          ForceOneFilePerPage = true
        }
      });
      Assert.AreEqual(2, results.Count(), "Wrong number of results");

      await AssertSinglePagePdfResultsAsync(results);
    }

    private async Task AssertSinglePagePdfResultsAsync(IEnumerable<Result> results, Action<string> customAssertions = null)
    {
      for (var i = 0; i < results.Count(); i++)
      {
        var result = results.ElementAt(i);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1, result.PageCount, "Wrong page count for result");

        var resultSourceDocument = result.Sources.ToList()[0];
        Assert.IsNotNull(resultSourceDocument.RemoteWorkFile);
        Assert.IsNull(resultSourceDocument.Password);
        Assert.AreEqual((i + 1).ToString(), resultSourceDocument.Pages, "Wrong source page range for result");

        var filename = $"page-{i}.pdf";
        await result.RemoteWorkFile.SaveAsync(filename);
        FileAssert.IsPdf(filename);

        if (customAssertions != null)
        {
          customAssertions(filename);
        }
      }
    }
  }
}
