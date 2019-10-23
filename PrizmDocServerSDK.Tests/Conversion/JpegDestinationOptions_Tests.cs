using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
  [TestClass]
  public class JpegDestinationOptions_Tests
  {
    [TestMethod]
    public void Can_easily_construct_with_no_options_set()
    {
      var jpegOptions = new JpegDestinationOptions();

      Assert.IsNull(jpegOptions.MaxWidth);
      Assert.IsNull(jpegOptions.MaxHeight);
    }

    [TestMethod]
    public void Can_easily_construct_with_only_MaxWidthInPixels()
    {
      var jpegOptions = new JpegDestinationOptions
      {
        MaxWidth = "600px"
      };

      Assert.AreEqual("600px", jpegOptions.MaxWidth);
      Assert.IsNull(jpegOptions.MaxHeight);
    }

    [TestMethod]
    public void Can_easily_construct_with_only_MaxHeightInPixels()
    {
      var jpegOptions = new JpegDestinationOptions
      {
        MaxHeight = "800px"
      };

      Assert.IsNull(jpegOptions.MaxWidth);
      Assert.AreEqual("800px", jpegOptions.MaxHeight);
    }

    [TestMethod]
    public void Can_easily_construct_with_both_MaxWidthInPixels_and_MaxHeightInPixels()
    {
      var jpegOptions = new JpegDestinationOptions
      {
        MaxWidth = "850px",
        MaxHeight = "1100px"
      };

      Assert.AreEqual("850px", jpegOptions.MaxWidth);
      Assert.AreEqual("1100px", jpegOptions.MaxHeight);
    }
  }
}
