using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accusoft.PrizmDocServer.Tests;
using Accusoft.PrizmDocServer.Exceptions;
using System.Collections.Generic;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
  [TestClass]
  public class Source_Pages_WithHeaderFooter_Tests
  {
    [TestMethod]
    public async Task When_applying_a_header_to_a_single_source_with_a_page_range_specified()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync(new SourceDocument("documents/example.pdf", pages: "2-"), new DestinationOptions(DestinationFileFormat.Pdf)
        {
          Header = new HeaderFooterOptions()
          {
            Lines = new List<HeaderFooterLine>()
            {
              new HeaderFooterLine() { Left = "Acme" }
            }
          }
        });
      }, "Remote server does not support applying headers or footers when \"pages\" is specified for a SourceDocument.");
    }

    [TestMethod]
    public async Task When_applying_a_footer_to_a_single_source_with_a_page_range_specified()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync(new SourceDocument("documents/example.pdf", pages: "2-"), new DestinationOptions(DestinationFileFormat.Pdf)
        {
          Footer = new HeaderFooterOptions()
          {
            Lines = new List<HeaderFooterLine>()
            {
              new HeaderFooterLine() { Left = "Acme" }
            }
          }
        });
      }, "Remote server does not support applying headers or footers when \"pages\" is specified for a SourceDocument.");
    }
  }
}
