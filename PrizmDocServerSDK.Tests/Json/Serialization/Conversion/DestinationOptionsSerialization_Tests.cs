using System.IO;
using Accusoft.PrizmDocServer.Conversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Json.Serialization.Conversion.Tests
{
    [TestClass]
    public class DestinationOptionsSerialization_Tests
    {
        [TestMethod]
        public void Can_serialize_a_minimal_instance()
        {
            var options = new DestinationOptions(DestinationFileFormat.Pdf);

            using (var stringWriter = new StringWriter())
            {
                PrizmDocRestApiJsonSerializer.Instance.Serialize(stringWriter, options);
                Assert.AreEqual("{\"format\":\"pdf\"}", stringWriter.ToString());
            }
        }

        [TestMethod]
        public void Can_serialize_custom_PDF_options()
        {
            var options = new DestinationOptions(DestinationFileFormat.Pdf)
            {
                PdfOptions = new PdfDestinationOptions
                {
                    ForceOneFilePerPage = true,
                    Ocr = new OcrOptions
                    {
                        DefaultDpi = new DpiOptions(72, 120),
                    },
                },
            };

            using (var stringWriter = new StringWriter())
            {
                PrizmDocRestApiJsonSerializer.Instance.Serialize(stringWriter, options);
                Assert.AreEqual("{\"format\":\"pdf\",\"pdfOptions\":{\"forceOneFilePerPage\":true,\"ocr\":{\"language\":\"english\",\"defaultDpi\":{\"x\":72,\"y\":120}}}}", stringWriter.ToString());
            }
        }

        [TestMethod]
        public void Can_serialize_full_set_of_options()
        {
            var options = new DestinationOptions(DestinationFileFormat.Pdf)
            {
                PdfOptions = new PdfDestinationOptions
                {
                    ForceOneFilePerPage = true,
                    Ocr = new OcrOptions
                    {
                        Language = "russian",
                        DefaultDpi = new DpiOptions(72, 120),
                    },
                },
                JpegOptions = new JpegDestinationOptions
                {
                    MaxWidth = "640px",
                    MaxHeight = "480px",
                },
                PngOptions = new PngDestinationOptions
                {
                    MaxWidth = "800px",
                    MaxHeight = "600px",
                },
            };

            using (var stringWriter = new StringWriter())
            {
                PrizmDocRestApiJsonSerializer.Instance.Serialize(stringWriter, options);
                Assert.AreEqual("{\"format\":\"pdf\",\"jpegOptions\":{\"maxWidth\":\"640px\",\"maxHeight\":\"480px\"},\"pdfOptions\":{\"forceOneFilePerPage\":true,\"ocr\":{\"language\":\"russian\",\"defaultDpi\":{\"x\":72,\"y\":120}}},\"pngOptions\":{\"maxWidth\":\"800px\",\"maxHeight\":\"600px\"}}", stringWriter.ToString());
            }
        }
    }
}
