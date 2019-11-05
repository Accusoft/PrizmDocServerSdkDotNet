using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class TiffDestinationOptions_Tests
    {
        [TestMethod]
        public void Can_easily_construct_with_no_options_set()
        {
            var tiffOptions = new TiffDestinationOptions();

            Assert.IsNull(tiffOptions.MaxWidth);
            Assert.IsNull(tiffOptions.MaxHeight);
        }

        [TestMethod]
        public void Can_easily_construct_with_only_ForceOneFilePerPage()
        {
            var tiffOptions = new TiffDestinationOptions
            {
                ForceOneFilePerPage = true,
            };

            Assert.IsTrue(tiffOptions.ForceOneFilePerPage);
            Assert.IsNull(tiffOptions.MaxWidth);
            Assert.IsNull(tiffOptions.MaxHeight);
        }

        [TestMethod]
        public void Can_easily_construct_with_only_MaxWidthInPixels()
        {
            var tiffOptions = new TiffDestinationOptions
            {
                MaxWidth = "600px",
            };

            Assert.IsFalse(tiffOptions.ForceOneFilePerPage);
            Assert.AreEqual("600px", tiffOptions.MaxWidth);
            Assert.IsNull(tiffOptions.MaxHeight);
        }

        [TestMethod]
        public void Can_easily_construct_with_only_MaxHeightInPixels()
        {
            var tiffOptions = new TiffDestinationOptions
            {
                MaxHeight = "800px",
            };

            Assert.IsFalse(tiffOptions.ForceOneFilePerPage);
            Assert.IsNull(tiffOptions.MaxWidth);
            Assert.AreEqual("800px", tiffOptions.MaxHeight);
        }

        [TestMethod]
        public void Can_easily_construct_with_both_MaxWidthInPixels_and_MaxHeightInPixels()
        {
            var tiffOptions = new TiffDestinationOptions
            {
                MaxWidth = "850px",
                MaxHeight = "1100px",
            };

            Assert.IsFalse(tiffOptions.ForceOneFilePerPage);
            Assert.AreEqual("850px", tiffOptions.MaxWidth);
            Assert.AreEqual("1100px", tiffOptions.MaxHeight);
        }
    }
}
