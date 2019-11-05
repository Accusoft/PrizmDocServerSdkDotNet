using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class DestinationOptions_Tests
    {
        [TestMethod]
        public void Can_easily_construct_a_minimal_instance()
        {
            var options = new DestinationOptions(DestinationFileFormat.Pdf);

            Assert.AreEqual(DestinationFileFormat.Pdf, options.Format);
            Assert.IsNull(options.PdfOptions);
            Assert.IsNull(options.JpegOptions);
            Assert.IsNull(options.PngOptions);
            Assert.IsNull(options.TiffOptions);
        }

        [TestMethod]
        public void Can_easily_construct_custom_options()
        {
            var options = new DestinationOptions(DestinationFileFormat.Pdf)
            {
                PdfOptions = new PdfDestinationOptions
                {
                    ForceOneFilePerPage = true,
                    Ocr = new OcrOptions
                    {
                        DefaultDpi = new DpiOptions(x: 72, y: 120),
                    },
                },
                Header = new HeaderFooterOptions
                {
                    Lines = new List<HeaderFooterLine>
                    {
                        new HeaderFooterLine { Left = "Accusoft", Right = "Page 1" },
                    },
                },
                Footer = new HeaderFooterOptions
                {
                    Color = "#FF0000",
                    Lines = new List<HeaderFooterLine>
                    {
                        new HeaderFooterLine { Center = "Center Info" },
                    },
                },
            };

            Assert.AreEqual(DestinationFileFormat.Pdf, options.Format);
            Assert.IsNotNull(options.PdfOptions);
            Assert.IsNull(options.JpegOptions);
            Assert.IsNull(options.PngOptions);
            Assert.IsNull(options.TiffOptions);
            Assert.IsNotNull(options.Header);
            Assert.IsNotNull(options.Footer);
        }
    }
}
