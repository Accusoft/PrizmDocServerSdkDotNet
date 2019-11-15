using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class ConvertAsync_Png_Tests
    {
        [TestMethod]
        public async Task With_local_file_path()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            IEnumerable<ConversionResult> results = await prizmDocServer.ConvertAsync("documents/example.docx", DestinationFileFormat.Png);
            Assert.AreEqual(2, results.Count(), "Wrong number of results");

            await this.AssertSinglePagePngResultsAsync(results);
        }

        [TestMethod]
        public async Task With_maxWidth_set_to_100px()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            IEnumerable<ConversionResult> results = await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Png)
            {
                PngOptions = new PngDestinationOptions()
                {
                    MaxWidth = "100px",
                },
            });
            Assert.AreEqual(2, results.Count(), "Wrong number of results");

            await this.AssertSinglePagePngResultsAsync(results, filename =>
            {
                ImageDimensions dimensions = ImageUtil.GetImageDimensions(filename);
                Assert.AreEqual(100, dimensions.Width);
            });
        }

        [TestMethod]
        public async Task With_maxHeight_set_to_150px()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            IEnumerable<ConversionResult> results = await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Png)
            {
                PngOptions = new PngDestinationOptions()
                {
                    MaxHeight = "150px",
                },
            });
            Assert.AreEqual(2, results.Count(), "Wrong number of results");

            await this.AssertSinglePagePngResultsAsync(results, filename =>
            {
                ImageDimensions dimensions = ImageUtil.GetImageDimensions(filename);
                Assert.AreEqual(150, dimensions.Height);
            });
        }

        [TestMethod]
        public async Task With_maxWidth_640px_and_maxHeight_480px()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            IEnumerable<ConversionResult> results = await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Png)
            {
                PngOptions = new PngDestinationOptions()
                {
                    MaxWidth = "640px",
                    MaxHeight = "480px",
                },
            });
            Assert.AreEqual(2, results.Count(), "Wrong number of results");

            await this.AssertSinglePagePngResultsAsync(results, filename =>
            {
                ImageDimensions dimensions = ImageUtil.GetImageDimensions(filename);
                Assert.IsTrue(dimensions.Width <= 640);
                Assert.IsTrue(dimensions.Height <= 480);
            });
        }

        private async Task AssertSinglePagePngResultsAsync(IEnumerable<ConversionResult> results, Action<string> customAssertions = null)
        {
            for (int i = 0; i < results.Count(); i++)
            {
                ConversionResult result = results.ElementAt(i);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(1, result.PageCount, "Wrong page count for result");

                ConversionSourceDocument resultSourceDocument = result.Sources.ToList()[0];
                Assert.IsNotNull(resultSourceDocument.RemoteWorkFile);
                Assert.IsNull(resultSourceDocument.Password);
                Assert.AreEqual((i + 1).ToString(), resultSourceDocument.Pages, "Wrong source page range for result");

                string filename = $"page-{i}.png";
                await result.RemoteWorkFile.SaveAsync(filename);
                FileAssert.IsPng(filename);

                if (customAssertions != null)
                {
                    customAssertions(filename);
                }
            }
        }
    }
}
