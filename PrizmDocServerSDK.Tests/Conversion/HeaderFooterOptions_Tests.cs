using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class HeaderFooterOptions_Tests
    {
        [TestMethod]
        public void Can_easily_construct_with_no_options_set()
        {
            var options = new HeaderFooterOptions();

            Assert.IsNull(options.Lines);
            Assert.IsNull(options.FontFamily);
            Assert.IsNull(options.FontSize);
            Assert.IsNull(options.Color);
        }

        [TestMethod]
        public void Can_easily_construct_with_all_options_set()
        {
            var lines = new List<HeaderFooterLine>
                {
                    new HeaderFooterLine { Left = "Acme, Inc.", Center = "Page 1 of 10", Right = "28 Aug 2019" },
                    new HeaderFooterLine { Center = "CONFIDENTIAL" },
                    new HeaderFooterLine { Left = "Acme" },
                    new HeaderFooterLine { Left = "123 Anvil St" },
                    new HeaderFooterLine { Left = "Somewhere, AZ 98765" },
                };

            var options = new HeaderFooterOptions
            {
                Lines = lines,
                FontFamily = "Lexicon",
                FontSize = "12pt",
                Color = "#FF0000",
            };

            Assert.AreEqual(lines, options.Lines);
            Assert.AreEqual("Lexicon", options.FontFamily);
            Assert.AreEqual("12pt", options.FontSize);
            Assert.AreEqual("#FF0000", options.Color);
        }
    }
}
