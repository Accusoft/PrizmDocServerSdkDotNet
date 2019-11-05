using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class Raster_BadMaxWidthOrMaxHeight_Tests
    {
        [TestMethod]
        public async Task JPEG_bad_MaxWidth()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Jpeg)
                    {
                        JpegOptions = new JpegDestinationOptions { MaxWidth = "wat" },
                    });
                }, $"Invalid JpegOptions.MaxWidth for remote server: \"wat\". Try using a CSS-style string, like \"800px\".");
        }

        [TestMethod]
        public async Task JPEG_bad_MaxHeight()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Jpeg)
                    {
                        JpegOptions = new JpegDestinationOptions { MaxHeight = "wat" },
                    });
                }, $"Invalid JpegOptions.MaxHeight for remote server: \"wat\". Try using a CSS-style string, like \"600px\".");
        }

        [TestMethod]
        public async Task PNG_bad_MaxWidth()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Png)
                    {
                        PngOptions = new PngDestinationOptions { MaxWidth = "wat" },
                    });
                }, $"Invalid PngOptions.MaxWidth for remote server: \"wat\". Try using a CSS-style string, like \"800px\".");
        }

        [TestMethod]
        public async Task PNG_bad_MaxHeight()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Png)
                    {
                        PngOptions = new PngDestinationOptions { MaxHeight = "wat" },
                    });
                }, $"Invalid PngOptions.MaxHeight for remote server: \"wat\". Try using a CSS-style string, like \"600px\".");
        }

        [TestMethod]
        public async Task TIFF_bad_MaxWidth()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Tiff)
                    {
                        TiffOptions = new TiffDestinationOptions { MaxWidth = "wat" },
                    });
                }, $"Invalid TiffOptions.MaxWidth for remote server: \"wat\". Try using a CSS-style string, like \"800px\".");
        }

        [TestMethod]
        public async Task TIFF_bad_MaxHeight()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Tiff)
                    {
                        TiffOptions = new TiffDestinationOptions { MaxHeight = "wat" },
                    });
                }, $"Invalid TiffOptions.MaxHeight for remote server: \"wat\". Try using a CSS-style string, like \"600px\".");
        }
    }
}
