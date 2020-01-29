using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class ConvertAsync_Svg_Tests
    {
        [TestMethod]
        public async Task With_local_file_path()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            IEnumerable<ConversionResult> results = await prizmDocServer.ConvertAsync("documents/example.docx", DestinationFileFormat.Svg);
            Assert.AreEqual(2, results.Count(), "Wrong number of results");

            await this.AssertSinglePageSvgResultsAsync(results);
        }

        private async Task AssertSinglePageSvgResultsAsync(IEnumerable<ConversionResult> results, Action<string> customAssertions = null)
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

                string filename = $"page-{i}.svg";
                await result.RemoteWorkFile.SaveAsync(filename);
                FileAssert.IsSvg(filename);

                customAssertions?.Invoke(filename);
            }
        }
    }
}
