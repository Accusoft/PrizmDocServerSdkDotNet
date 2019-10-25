using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accusoft.PrizmDocServer.Tests;
using Accusoft.PrizmDocServer.Exceptions;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
  [TestClass]
  public class RasterizingCadWithoutMaxWidthOrHeight_Tests
  {
    [TestMethod]
    public async Task CAD_to_PNG()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync(new SourceDocument("documents/example.dwg"), new DestinationOptions(DestinationFileFormat.Png));
      }, "When converting a CAD SourceDocument to PNG, you must specify PngOptions.MaxWidth or PngOptions.MaxHeight.");
    }

    [TestMethod]
    public async Task CAD_to_JPEG()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync(new SourceDocument("documents/example.dwg"), new DestinationOptions(DestinationFileFormat.Jpeg));
      }, "When converting a CAD SourceDocument to JPEG, you must specify JpegOptions.MaxWidth or JpegOptions.MaxHeight.");
    }

    [TestMethod]
    public async Task CAD_to_TIFF()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync(new SourceDocument("documents/example.dwg"), new DestinationOptions(DestinationFileFormat.Tiff));
      }, "When converting a CAD SourceDocument to TIFF, you must specify TiffOptions.MaxWidth or TiffOptions.MaxHeight.");
    }
  }
}
