using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using System.Collections.Generic;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
  [TestClass]
  public class HeaderFooter_UnavailableFontFamily_Tests
  {
    [TestMethod]
    public async Task Unavailable_font_family_with_header()
    {
      var context = Util.CreateContext();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await context.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
        {
          Header = new HeaderFooterOptions
          {
            FontFamily = "Lexicon",
            Lines = new List<HeaderFooterLine>
            {
              new HeaderFooterLine { Left = "Test" },
            }
          }
        });
      }, "Header.FontFamily or Footer.FontFamily specifies a font which is not available on the remote server.");
    }

    [TestMethod]
    public async Task Unavailable_font_family_with_footer()
    {
      var context = Util.CreateContext();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await context.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
        {
          Footer = new HeaderFooterOptions
          {
            FontFamily = "Lexicon",
            Lines = new List<HeaderFooterLine>
            {
              new HeaderFooterLine { Left = "Test" },
            }
          }
        });
      }, "Header.FontFamily or Footer.FontFamily specifies a font which is not available on the remote server.");
    }
  }
}
