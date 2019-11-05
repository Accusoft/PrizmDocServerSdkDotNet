using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class ConvertAsync_Pdf_SinglePage_Tests
    {
        [TestMethod]
        public async Task With_local_file_path()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            IEnumerable<Result> results = await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
            {
                PdfOptions = new PdfDestinationOptions()
                {
                    ForceOneFilePerPage = true,
                },
            });
            Assert.AreEqual(2, results.Count(), "Wrong number of results");

            await this.AssertSinglePagePdfResultsAsync(results);
        }

        private async Task AssertSinglePagePdfResultsAsync(IEnumerable<Result> results, Action<string> customAssertions = null)
        {
            for (int i = 0; i < results.Count(); i++)
            {
                Result result = results.ElementAt(i);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(1, result.PageCount, "Wrong page count for result");

                SourceDocument resultSourceDocument = result.Sources.ToList()[0];
                Assert.IsNotNull(resultSourceDocument.RemoteWorkFile);
                Assert.IsNull(resultSourceDocument.Password);
                Assert.AreEqual((i + 1).ToString(), resultSourceDocument.Pages, "Wrong source page range for result");

                var filename = $"page-{i}.pdf";
                await result.RemoteWorkFile.SaveAsync(filename);
                FileAssert.IsPdf(filename);

                if (customAssertions != null)
                {
                    customAssertions(filename);
                }
            }
        }
    }
}
