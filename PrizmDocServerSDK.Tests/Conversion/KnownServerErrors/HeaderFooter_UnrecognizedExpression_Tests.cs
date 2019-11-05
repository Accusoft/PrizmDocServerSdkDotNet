using System.Collections.Generic;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class HeaderFooter_UnrecognizedExpression_Tests
    {
        [TestMethod]
        public async Task Header_line_0_left()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Header = new HeaderFooterOptions
                        {
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Page {{wat}}" },
                            },
                        },
                    });
                }, "Remote server rejected Header.Lines[0].Left because it contains a dynamic expression (text in double curly braces) that it did not recognize.");
        }

        [TestMethod]
        public async Task Footer_line_0_left()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Footer = new HeaderFooterOptions
                        {
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Page {{wat}}" },
                            },
                        },
                    });
                }, "Remote server rejected Footer.Lines[0].Left because it contains a dynamic expression (text in double curly braces) that it did not recognize.");
        }

        [TestMethod]
        public async Task Header_line_0_center()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Header = new HeaderFooterOptions
                        {
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Center = "Page {{wat}}" },
                            },
                        },
                    });
                }, "Remote server rejected Header.Lines[0].Center because it contains a dynamic expression (text in double curly braces) that it did not recognize.");
        }

        [TestMethod]
        public async Task Footer_line_0_center()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Footer = new HeaderFooterOptions
                        {
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Center = "Page {{wat}}" },
                            },
                        },
                    });
                }, "Remote server rejected Footer.Lines[0].Center because it contains a dynamic expression (text in double curly braces) that it did not recognize.");
        }

        [TestMethod]
        public async Task Header_line_0_right()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Header = new HeaderFooterOptions
                        {
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Right = "Page {{wat}}" },
                            },
                        },
                    });
                }, "Remote server rejected Header.Lines[0].Right because it contains a dynamic expression (text in double curly braces) that it did not recognize.");
        }

        [TestMethod]
        public async Task Footer_line_0_right()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Footer = new HeaderFooterOptions
                        {
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Right = "Page {{wat}}" },
                            },
                        },
                    });
                }, "Remote server rejected Footer.Lines[0].Right because it contains a dynamic expression (text in double curly braces) that it did not recognize.");
        }

        [TestMethod]
        public async Task Header_line_1_left()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Header = new HeaderFooterOptions
                        {
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Acme" },
                                new HeaderFooterLine { Left = "Page {{wat}}" },
                            },
                        },
                    });
                }, "Remote server rejected Header.Lines[1].Left because it contains a dynamic expression (text in double curly braces) that it did not recognize.");
        }

        [TestMethod]
        public async Task Footer_line_1_left()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Footer = new HeaderFooterOptions
                        {
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Acme" },
                                new HeaderFooterLine { Left = "Page {{wat}}" },
                            },
                        },
                    });
                }, "Remote server rejected Footer.Lines[1].Left because it contains a dynamic expression (text in double curly braces) that it did not recognize.");
        }

        [TestMethod]
        public async Task Header_line_1_center()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Header = new HeaderFooterOptions
                        {
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Acme" },
                                new HeaderFooterLine { Center = "Page {{wat}}" },
                            },
                        },
                    });
                }, "Remote server rejected Header.Lines[1].Center because it contains a dynamic expression (text in double curly braces) that it did not recognize.");
        }

        [TestMethod]
        public async Task Footer_line_1_center()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Footer = new HeaderFooterOptions
                        {
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Acme" },
                                new HeaderFooterLine { Center = "Page {{wat}}" },
                            },
                        },
                    });
                }, "Remote server rejected Footer.Lines[1].Center because it contains a dynamic expression (text in double curly braces) that it did not recognize.");
        }

        [TestMethod]
        public async Task Header_line_1_right()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Header = new HeaderFooterOptions
                        {
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Acme" },
                                new HeaderFooterLine { Right = "Page {{wat}}" },
                            },
                        },
                    });
                }, "Remote server rejected Header.Lines[1].Right because it contains a dynamic expression (text in double curly braces) that it did not recognize.");
        }

        [TestMethod]
        public async Task Footer_line_1_right()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync("documents/example.docx", new DestinationOptions(DestinationFileFormat.Pdf)
                    {
                        Footer = new HeaderFooterOptions
                        {
                            Lines = new List<HeaderFooterLine>
                            {
                                new HeaderFooterLine { Left = "Acme" },
                                new HeaderFooterLine { Right = "Page {{wat}}" },
                            },
                        },
                    });
                }, "Remote server rejected Footer.Lines[1].Right because it contains a dynamic expression (text in double curly braces) that it did not recognize.");
        }
    }
}
