using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class RasterizingCadWithoutMaxWidthOrHeight_Tests
    {
        [TestMethod]
        public async Task CAD_to_PNG()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(new ConversionSourceDocument("documents/example.dwg"), new DestinationOptions(DestinationFileFormat.Png));
                }, "When converting a CAD ConversionSourceDocument to PNG, you must specify PngOptions.MaxWidth or PngOptions.MaxHeight.");
        }

        [TestMethod]
        public async Task CAD_to_JPEG()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(new ConversionSourceDocument("documents/example.dwg"), new DestinationOptions(DestinationFileFormat.Jpeg));
                }, "When converting a CAD ConversionSourceDocument to JPEG, you must specify JpegOptions.MaxWidth or JpegOptions.MaxHeight.");
        }

        [TestMethod]
        public async Task CAD_to_TIFF()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(new ConversionSourceDocument("documents/example.dwg"), new DestinationOptions(DestinationFileFormat.Tiff));
                }, "When converting a CAD ConversionSourceDocument to TIFF, you must specify TiffOptions.MaxWidth or TiffOptions.MaxHeight.");
        }
    }
}
