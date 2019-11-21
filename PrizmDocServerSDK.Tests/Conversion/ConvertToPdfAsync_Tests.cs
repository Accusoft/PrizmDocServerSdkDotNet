using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class ConvertToPdfAsync_Tests
    {
        [TestMethod]
        public async Task With_local_file_path()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionResult result = await prizmDocServer.ConvertToPdfAsync("documents/example.docx");
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.PageCount);

            ConversionSourceDocument resultSourceDocument = result.Sources.ToList()[0];
            Assert.IsNotNull(resultSourceDocument.RemoteWorkFile);
            Assert.IsNull(resultSourceDocument.Password);
            Assert.AreEqual("1-2", resultSourceDocument.Pages);

            await result.RemoteWorkFile.SaveAsync("output.pdf");
            FileAssert.IsPdf("output.pdf");
        }

        [TestMethod]
        public async Task Just_the_first_page()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionSourceDocument sourceDocument = new ConversionSourceDocument("documents/example.docx", pages: "1");
            ConversionResult result = await prizmDocServer.ConvertToPdfAsync(sourceDocument);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.PageCount);

            ConversionSourceDocument resultSourceDocument = result.Sources.ToList()[0];
            Assert.AreEqual(sourceDocument.RemoteWorkFile, resultSourceDocument.RemoteWorkFile);
            Assert.IsNull(resultSourceDocument.Password);
            Assert.AreEqual("1", resultSourceDocument.Pages);

            await result.RemoteWorkFile.SaveAsync("output.pdf");
            FileAssert.IsPdf("output.pdf");
        }

        [TestMethod]
        public async Task With_header()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionResult result = await prizmDocServer.ConvertToPdfAsync("documents/example.docx", header: new HeaderFooterOptions()
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
            });

            string[] pagesText = await TextUtil.ExtractPagesText(result.RemoteWorkFile);
            foreach (string page in pagesText)
            {
                StringAssert.Contains(page, "Top Left");
                StringAssert.Contains(page, "THIS IS HEADER CONTENT");
                StringAssert.Contains(page, "Top Right");
            }
        }

        [TestMethod]
        public async Task With_footer()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionResult result = await prizmDocServer.ConvertToPdfAsync("documents/example.docx", footer: new HeaderFooterOptions()
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
            });

            string[] pagesText = await TextUtil.ExtractPagesText(result.RemoteWorkFile);
            foreach (string page in pagesText)
            {
                StringAssert.Contains(page, "Bottom Left");
                StringAssert.Contains(page, "THIS IS FOOTER CONTENT");
                StringAssert.Contains(page, "Bottom Right");
            }
        }

        [TestMethod]
        public async Task With_header_and_footer()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionResult result = await prizmDocServer.ConvertToPdfAsync(
                "documents/example.docx",
                header: new HeaderFooterOptions()
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
                footer: new HeaderFooterOptions()
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
                });

            string[] pagesText = await TextUtil.ExtractPagesText(result.RemoteWorkFile);
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
