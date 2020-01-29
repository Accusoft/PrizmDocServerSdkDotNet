using System.Collections.Generic;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class HeaderFooter_UnsupportedDestinationFormat_Tests
    {
        private readonly HeaderFooterOptions exampleHeaderFooterContent = new HeaderFooterOptions
        {
            Lines = new List<HeaderFooterLine>
            {
                new HeaderFooterLine { Left = "Test" },
            },
        };

        [TestMethod]
        public async Task Header_with_SVG_output()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Svg) { Header = this.exampleHeaderFooterContent });
                }, "Remote server does not support applying headers or footers when producing SVG output.");
        }

        [TestMethod]
        public async Task Footer_with_SVG_output()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Svg) { Footer = this.exampleHeaderFooterContent });
                }, "Remote server does not support applying headers or footers when producing SVG output.");
        }

        [TestMethod]
        public async Task Header_with_PNG_output()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Png) { Header = this.exampleHeaderFooterContent });
                }, "Remote server does not support applying headers or footers when producing PNG output.");
        }

        [TestMethod]
        public async Task Footer_with_PNG_output()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Png) { Footer = this.exampleHeaderFooterContent });
                }, "Remote server does not support applying headers or footers when producing PNG output.");
        }

        [TestMethod]
        public async Task Header_with_JPEG_output()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Jpeg) { Header = this.exampleHeaderFooterContent });
                }, "Remote server does not support applying headers or footers when producing JPEG output.");
        }

        [TestMethod]
        public async Task Footer_with_JPEG_output()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Jpeg) { Footer = this.exampleHeaderFooterContent });
                }, "Remote server does not support applying headers or footers when producing JPEG output.");
        }

        [PdcTestMethod]
        public async Task Header_with_DOCX_output()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Docx) { Header = this.exampleHeaderFooterContent });
                }, "Remote server does not support applying headers or footers when producing DOCX output.");
        }

        [PdcTestMethod]
        public async Task Footer_with_DOCX_output()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Docx) { Footer = this.exampleHeaderFooterContent });
                }, "Remote server does not support applying headers or footers when producing DOCX output.");
        }
    }
}
