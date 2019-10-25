using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accusoft.PrizmDocServer.Tests;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
  [TestClass]
  public class ConvertAsync_Jpeg_Tests
  {
    [TestMethod]
    public async Task With_local_file_path()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();
      var results = await prizmDocServer.ConvertAsync("documents/example.docx", DestinationFileFormat.Jpeg);
      Assert.AreEqual(2, results.Count(), "Wrong number of results");

      await AssertSinglePageJpegResultsAsync(results);
    }

    [TestMethod]
    public async Task With_maxWidth_set_to_100px()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();
      var results = await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Jpeg)
      {
        JpegOptions = new JpegDestinationOptions()
        {
          MaxWidth = "100px"
        }
      });
      Assert.AreEqual(2, results.Count(), "Wrong number of results");

      await AssertSinglePageJpegResultsAsync(results, filename =>
      {
        var dimensions = ImageUtil.GetImageDimensions(filename);
        Assert.AreEqual(100, dimensions.Width);
      });
    }

    [TestMethod]
    public async Task With_maxHeight_set_to_150px()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();
      var results = await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Jpeg)
      {
        JpegOptions = new JpegDestinationOptions()
        {
          MaxHeight = "150px"
        }
      });
      Assert.AreEqual(2, results.Count(), "Wrong number of results");

      await AssertSinglePageJpegResultsAsync(results, filename =>
      {
        var dimensions = ImageUtil.GetImageDimensions(filename);
        Assert.AreEqual(150, dimensions.Height);
      });
    }

    [TestMethod]
    public async Task With_maxWidth_640px_and_maxHeight_480px()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();
      var results = await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Jpeg)
      {
        JpegOptions = new JpegDestinationOptions()
        {
          MaxWidth = "640px",
          MaxHeight = "480px"
        }
      });
      Assert.AreEqual(2, results.Count(), "Wrong number of results");

      await AssertSinglePageJpegResultsAsync(results, filename =>
      {
        var dimensions = ImageUtil.GetImageDimensions(filename);
        Assert.IsTrue(dimensions.Width <= 640);
        Assert.IsTrue(dimensions.Height <= 480);
      });
    }

    private async Task AssertSinglePageJpegResultsAsync(IEnumerable<Result> results, Action<string> customAssertions = null)
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

        var filename = $"page-{i}.jpg";
        await result.RemoteWorkFile.SaveAsync(filename);
        FileAssert.IsJpeg(filename);

        if (customAssertions != null)
        {
          customAssertions(filename);
        }
      }
    }
  }
}
