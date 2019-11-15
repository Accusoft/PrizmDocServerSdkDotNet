using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class ConvertToTiffAsync_Tests
    {
        [TestMethod]
        public async Task With_local_file_path()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionResult result = (await prizmDocServer.ConvertAsync("documents/example.docx", DestinationFileFormat.Tiff)).Single();
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.PageCount);

            ConversionSourceDocument resultSourceDocument = result.Sources.ToList()[0];
            Assert.IsNotNull(resultSourceDocument.RemoteWorkFile);
            Assert.IsNull(resultSourceDocument.Password);
            Assert.AreEqual("1-2", resultSourceDocument.Pages);

            await result.RemoteWorkFile.SaveAsync("output.tiff");
            FileAssert.IsTiff("output.tiff");
        }

        [TestMethod]
        public async Task Just_the_first_page()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionSourceDocument sourceDocument = new ConversionSourceDocument("documents/example.docx", pages: "1");
            ConversionResult result = (await prizmDocServer.ConvertAsync(sourceDocument, DestinationFileFormat.Tiff)).Single();
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.PageCount);

            ConversionSourceDocument resultSourceDocument = result.Sources.ToList()[0];
            Assert.AreEqual(sourceDocument.RemoteWorkFile, resultSourceDocument.RemoteWorkFile);
            Assert.IsNull(resultSourceDocument.Password);
            Assert.AreEqual("1", resultSourceDocument.Pages);

            await result.RemoteWorkFile.SaveAsync("output.tiff");
            FileAssert.IsTiff("output.tiff");
        }

        [TestMethod]
        public async Task With_maxWidth_set_to_100px()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionResult result = (await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Tiff)
            {
                TiffOptions = new TiffDestinationOptions()
                {
                    MaxWidth = "100px",
                },
            })).Single();
            await result.RemoteWorkFile.SaveAsync("output.tiff");
            IEnumerable<ImageDimensions> pagesDimensions = await ImageUtil.GetTiffPagesDimensionsAsync("output.tiff");
            foreach (ImageDimensions dimensions in pagesDimensions)
            {
                Assert.AreEqual(100, dimensions.Width);
            }
        }

        [TestMethod]
        public async Task With_maxHeight_set_to_150px()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionResult result = (await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Tiff)
            {
                TiffOptions = new TiffDestinationOptions()
                {
                    MaxHeight = "150px",
                },
            })).Single();
            await result.RemoteWorkFile.SaveAsync("output.tiff");
            IEnumerable<ImageDimensions> pagesDimensions = await ImageUtil.GetTiffPagesDimensionsAsync("output.tiff");
            foreach (ImageDimensions dimensions in pagesDimensions)
            {
                Assert.AreEqual(150, dimensions.Height);
            }
        }

        [TestMethod]
        public async Task With_maxWidth_640px_and_maxHeight_480px()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionResult result = (await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Tiff)
            {
                TiffOptions = new TiffDestinationOptions()
                {
                    MaxWidth = "640px",
                    MaxHeight = "480px",
                },
            })).Single();
            await result.RemoteWorkFile.SaveAsync("output.tiff");
            IEnumerable<ImageDimensions> pagesDimensions = await ImageUtil.GetTiffPagesDimensionsAsync("output.tiff");
            foreach (ImageDimensions dimensions in pagesDimensions)
            {
                Assert.IsTrue(dimensions.Width <= 640);
                Assert.IsTrue(dimensions.Height <= 480);
            }
        }

        [TestMethod]
        [TestCategory("Slow")]
        public async Task With_header()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionResult result = (await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Tiff)
            {
                Header = new HeaderFooterOptions()
                {
                    Lines = new List<HeaderFooterLine>()
                    {
                        new HeaderFooterLine()
                        {
                            Left = "Top Left",
                            Center = "THIS IS HEADER CONTENT",
                            Right = "Top Right",
                        },
                    },
                },
            })).Single();

            result = await prizmDocServer.OcrToPdfAsync(new ConversionSourceDocument(result.RemoteWorkFile));
            IEnumerable<string> pagesText = await TextUtil.ExtractPagesText(result.RemoteWorkFile);
            foreach (string page in pagesText)
            {
                StringAssert.Contains(page, "Top Left");
                StringAssert.Contains(page, "THIS IS HEADER CONTENT");
                StringAssert.Contains(page, "Top Right");
            }
        }

        [TestMethod]
        [TestCategory("Slow")]
        public async Task With_footer()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionResult result = (await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Tiff)
            {
                Footer = new HeaderFooterOptions()
                {
                    Lines = new List<HeaderFooterLine>()
                    {
                        new HeaderFooterLine()
                        {
                            Left = "Bottom Left",
                            Center = "THIS IS FOOTER CONTENT",
                            Right = "Bottom Right",
                        },
                    },
                },
            })).Single();

            result = await prizmDocServer.OcrToPdfAsync(new ConversionSourceDocument(result.RemoteWorkFile));
            IEnumerable<string> pagesText = await TextUtil.ExtractPagesText(result.RemoteWorkFile);
            foreach (string page in pagesText)
            {
                StringAssert.Contains(page, "Bottom Left");
                StringAssert.Contains(page, "THIS IS FOOTER CONTENT");
                StringAssert.Contains(page, "Bottom Right");
            }
        }

        [TestMethod]
        [TestCategory("Slow")]
        public async Task With_header_and_footer()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionResult result = (await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Tiff)
            {
                Header = new HeaderFooterOptions()
                {
                    Lines = new List<HeaderFooterLine>()
                    {
                        new HeaderFooterLine()
                        {
                            Left = "Top Left",
                            Center = "THIS IS HEADER CONTENT",
                            Right = "Top Right",
                        },
                    },
                },
                Footer = new HeaderFooterOptions()
                {
                    Lines = new List<HeaderFooterLine>()
                    {
                        new HeaderFooterLine()
                        {
                            Left = "Bottom Left",
                            Center = "THIS IS FOOTER CONTENT",
                            Right = "Bottom Right",
                        },
                    },
                },
            })).Single();

            result = await prizmDocServer.OcrToPdfAsync(new ConversionSourceDocument(result.RemoteWorkFile));
            IEnumerable<string> pagesText = await TextUtil.ExtractPagesText(result.RemoteWorkFile);
            foreach (string page in pagesText)
            {
                StringAssert.Contains(page, "Top Left");
                StringAssert.Contains(page, "THIS IS HEADER CONTENT");
                StringAssert.Contains(page, "Top Right");
                StringAssert.Contains(page, "Bottom Left");
                StringAssert.Contains(page, "THIS IS FOOTER CONTENT");
                StringAssert.Contains(page, "Bottom Right");
            }
        }
    }
}
