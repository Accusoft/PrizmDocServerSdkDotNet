using System.Collections.Generic;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class HeaderFooter_ForceOneFilePerPageNotSupported_Tests
    {
        private HeaderFooterOptions exampleHeaderFooterContent = new HeaderFooterOptions
        {
            Lines = new List<HeaderFooterLine>
            {
                new HeaderFooterLine { Left = "Test" },
            },
        };

        [TestMethod]
        public async Task Header_with_PDF_output_with_force_one_file_per_page()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        PdfOptions = new PdfDestinationOptions { ForceOneFilePerPage = true },
                        Header = this.exampleHeaderFooterContent,
                    });
                }, "Remote server does not support applying headers or footers when PdfOptions.ForceOneFilePerPage is set to true.");
        }

        [TestMethod]
        public async Task Footer_with_PDF_output_with_force_one_file_per_page()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        PdfOptions = new PdfDestinationOptions { ForceOneFilePerPage = true },
                        Footer = this.exampleHeaderFooterContent,
                    });
                }, "Remote server does not support applying headers or footers when PdfOptions.ForceOneFilePerPage is set to true.");
        }

        [TestMethod]
        public async Task Header_with_TIFF_output_with_force_one_file_per_page()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Tiff)
                    {
                        TiffOptions = new TiffDestinationOptions { ForceOneFilePerPage = true },
                        Header = this.exampleHeaderFooterContent,
                    });
                }, "Remote server does not support applying headers or footers when TiffOptions.ForceOneFilePerPage is set to true.");
        }

        [TestMethod]
        public async Task Footer_with_TIFF_output_with_force_one_file_per_page()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.pdf", new DestinationOptions(DestinationFileFormat.Tiff)
                    {
                        TiffOptions = new TiffDestinationOptions { ForceOneFilePerPage = true },
                        Footer = this.exampleHeaderFooterContent,
                    });
                }, "Remote server does not support applying headers or footers when TiffOptions.ForceOneFilePerPage is set to true.");
        }
    }
}
