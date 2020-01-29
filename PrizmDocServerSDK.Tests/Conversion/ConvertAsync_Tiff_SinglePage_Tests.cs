using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class ConvertAsync_Tiff_SinglePage_Tests
    {
        [TestMethod]
        public async Task With_local_file_path()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            IEnumerable<ConversionResult> results = await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Tiff)
            {
                TiffOptions = new TiffDestinationOptions()
                {
                    ForceOneFilePerPage = true,
                },
            });
            Assert.AreEqual(2, results.Count(), "Wrong number of results");

            await this.AssertSinglePageTiffResultsAsync(results);
        }

        [TestMethod]
        public async Task With_maxWidth_set_to_100px()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            IEnumerable<ConversionResult> results = await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Tiff)
            {
                TiffOptions = new TiffDestinationOptions()
                {
                    ForceOneFilePerPage = true,
                    MaxWidth = "100px",
                },
            });
            Assert.AreEqual(2, results.Count(), "Wrong number of results");

            await this.AssertSinglePageTiffResultsAsync(results, async filename =>
            {
                ImageDimensions dimensions = (await ImageUtil.GetTiffPagesDimensionsAsync(filename)).Single();
                Assert.AreEqual(100, dimensions.Width);
            });
        }

        [TestMethod]
        public async Task With_maxHeight_set_to_150px()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            IEnumerable<ConversionResult> results = await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Tiff)
            {
                TiffOptions = new TiffDestinationOptions()
                {
                    ForceOneFilePerPage = true,
                    MaxHeight = "150px",
                },
            });
            Assert.AreEqual(2, results.Count(), "Wrong number of results");

            await this.AssertSinglePageTiffResultsAsync(results, async filename =>
            {
                ImageDimensions dimensions = (await ImageUtil.GetTiffPagesDimensionsAsync(filename)).Single();
                Assert.AreEqual(150, dimensions.Height);
            });
        }

        [TestMethod]
        public async Task With_maxWidth_640px_and_maxHeight_480px()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            IEnumerable<ConversionResult> results = await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Tiff)
            {
                TiffOptions = new TiffDestinationOptions()
                {
                    ForceOneFilePerPage = true,
                    MaxWidth = "640px",
                    MaxHeight = "480px",
                },
            });
            Assert.AreEqual(2, results.Count(), "Wrong number of results");

            await this.AssertSinglePageTiffResultsAsync(results, async filename =>
            {
                ImageDimensions dimensions = (await ImageUtil.GetTiffPagesDimensionsAsync(filename)).Single();
                Assert.IsTrue(dimensions.Width <= 640);
                Assert.IsTrue(dimensions.Height <= 480);
            });
        }

        private async Task AssertSinglePageTiffResultsAsync(IEnumerable<ConversionResult> results, Action<string> customAssertions = null)
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

                string filename = $"page-{i}.tiff";
                await result.RemoteWorkFile.SaveAsync(filename);
                FileAssert.IsTiff(filename);

                customAssertions?.Invoke(filename);
            }
        }
    }
}
