using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
  [TestClass]
  public class OcrOptions_Tests
  {
    [TestMethod]
    public void Can_easily_construct_with_no_options_set()
    {
      var options = new OcrOptions();

      Assert.AreEqual("english", options.Language);
      Assert.IsNull(options.DefaultDpi);
    }

    [TestMethod]
    public void Can_easily_construct_with_custom_default_dpi()
    {
      var options = new OcrOptions
      {
        DefaultDpi = new DpiOptions(x: 72, y: 120)
      };

      Assert.IsNotNull(options.DefaultDpi);
      Assert.AreEqual(72, options.DefaultDpi.X);
      Assert.AreEqual(120, options.DefaultDpi.Y);
    }

    [TestMethod]
    public void Can_construct_with_a_custom_language()
    {
      var options = new OcrOptions
      {
        Language = "russian"
      };

      Assert.AreEqual("russian", options.Language);
      Assert.IsNull(options.DefaultDpi);
    }
  }
}
