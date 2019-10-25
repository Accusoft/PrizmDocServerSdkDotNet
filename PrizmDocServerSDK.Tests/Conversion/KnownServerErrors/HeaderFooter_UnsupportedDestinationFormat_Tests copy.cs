using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using System.Collections.Generic;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
  [TestClass]
  public class HeaderFooter_UnsupportedDestinationFormat_Tests
  {
    HeaderFooterOptions exampleHeaderFooterContent = new HeaderFooterOptions
    {
      Lines = new List<HeaderFooterLine>
      {
        new HeaderFooterLine { Left = "Test" },
      }
    };

    [TestMethod]
    public async Task Header_with_SVG_output()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Svg) { Header = exampleHeaderFooterContent });
      }, "Remote server does not support applying headers or footers when producing SVG output.");
    }

    [TestMethod]
    public async Task Footer_with_SVG_output()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Svg) { Footer = exampleHeaderFooterContent });
      }, "Remote server does not support applying headers or footers when producing SVG output.");
    }

    [TestMethod]
    public async Task Header_with_PNG_output()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Png) { Header = exampleHeaderFooterContent });
      }, "Remote server does not support applying headers or footers when producing PNG output.");
    }

    [TestMethod]
    public async Task Footer_with_PNG_output()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Png) { Footer = exampleHeaderFooterContent });
      }, "Remote server does not support applying headers or footers when producing PNG output.");
    }

    [TestMethod]
    public async Task Header_with_JPEG_output()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Jpeg) { Header = exampleHeaderFooterContent });
      }, "Remote server does not support applying headers or footers when producing JPEG output.");
    }

    [TestMethod]
    public async Task Footer_with_JPEG_output()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Jpeg) { Footer = exampleHeaderFooterContent });
      }, "Remote server does not support applying headers or footers when producing JPEG output.");
    }

    [PdcTestMethod]
    public async Task Header_with_DOCX_output()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Docx) { Header = exampleHeaderFooterContent });
      }, "Remote server does not support applying headers or footers when producing DOCX output.");
    }

    [PdcTestMethod]
    public async Task Footer_with_DOCX_output()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Docx) { Footer = exampleHeaderFooterContent });
      }, "Remote server does not support applying headers or footers when producing DOCX output.");
    }
  }
}
