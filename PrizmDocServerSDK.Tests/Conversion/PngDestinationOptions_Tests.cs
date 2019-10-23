using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
  [TestClass]
  public class PngDestinationOptions_Tests
  {
    [TestMethod]
    public void Can_easily_construct_with_no_options_set()
    {
      var pngOptions = new PngDestinationOptions();

      Assert.IsNull(pngOptions.MaxWidth);
      Assert.IsNull(pngOptions.MaxHeight);
    }

    [TestMethod]
    public void Can_easily_construct_with_only_MaxWidthInPixels()
    {
      var pngOptions = new PngDestinationOptions
      {
        MaxWidth = "600px"
      };

      Assert.AreEqual("600px", pngOptions.MaxWidth);
      Assert.IsNull(pngOptions.MaxHeight);
    }

    [TestMethod]
    public void Can_easily_construct_with_only_MaxHeightInPixels()
    {
      var pngOptions = new PngDestinationOptions
      {
        MaxHeight = "800px"
      };

      Assert.IsNull(pngOptions.MaxWidth);
      Assert.AreEqual("800px", pngOptions.MaxHeight);
    }

    [TestMethod]
    public void Can_easily_construct_with_both_MaxWidthInPixels_and_MaxHeightInPixels()
    {
      var pngOptions = new PngDestinationOptions
      {
        MaxWidth = "850px",
        MaxHeight = "1100px"
      };

      Assert.AreEqual("850px", pngOptions.MaxWidth);
      Assert.AreEqual("1100px", pngOptions.MaxHeight);
    }
  }
}
