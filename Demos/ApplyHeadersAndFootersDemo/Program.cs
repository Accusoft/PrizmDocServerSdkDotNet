using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer;
using Accusoft.PrizmDocServer.Conversion;

namespace Demos
{
    /// <summary>
    /// Demo program which applies headers and footers to a document.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            File.Delete("output.pdf");

            var prizmDocServer = new PrizmDocServerClient(Environment.GetEnvironmentVariable("BASE_URL"), Environment.GetEnvironmentVariable("API_KEY"));

            // Take a DOCX file, append headers and footers to each page (expanding
            // the page size), and convert it to a PDF.
            var result = await prizmDocServer.ConvertToPdfAsync(
                "project-proposal.docx",
                header: new HeaderFooterOptions
                {
                    Color = "#0000FF", // blue
                    Lines = new List<HeaderFooterLine>
                    {
                        new HeaderFooterLine { Left = "Top Left", Center = "Top", Right = "Top Right" },
                        new HeaderFooterLine { Center = "Page {{pageNumber}} of {{pageCount}}" },
                    },
                },
                footer: new HeaderFooterOptions
                {
                    Color = "#FF0000", // red
                    Lines = new List<HeaderFooterLine>
                    {
                        new HeaderFooterLine { Center = "BATES{{pageNumber+4000,10}}" },
                        new HeaderFooterLine { Left = "Bottom Left", Center = "Bottom", Right = "Bottom Right" },
                    },
                });

            // Save the result to "output.pdf".
            await result.RemoteWorkFile.SaveAsync("output.pdf");
        }
    }
}
