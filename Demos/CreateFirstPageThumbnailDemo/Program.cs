using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer;
using Accusoft.PrizmDocServer.Conversion;

namespace Demos
{
    /// <summary>
    /// Demo program which creates a thumbnail image for the first page of a document.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            File.Delete("thumbnail.png");

            var prizmDocServer = new PrizmDocServerClient(Environment.GetEnvironmentVariable("BASE_URL"), Environment.GetEnvironmentVariable("API_KEY"));

            // PrizmDoc Server does not currently allow you to extract pages
            // and convert to PNG in a single operation. But you can still get
            // a first-page thumbnail efficiently as a two-step process:
            //
            // 1. Extract just the first page as a PDF (which you never need to download)
            // 2. Convert that PDF to a thumbnail PNG

            // Extract the first page as an intermediate PDF. We won't ever bother
            // downloading this from PrizmDoc Server.
            ConversionResult tempFirstPagePdf = await prizmDocServer.ConvertToPdfAsync(new ConversionSourceDocument("project-proposal.docx", pages: "1"));

            // Convert the PDF to PNGs, specifying a max width and height. We'll get
            // back a collection of results, one per page. In our case, there is only
            // one page.
            IEnumerable<ConversionResult> thumbnailPngs = await prizmDocServer.ConvertAsync(new ConversionSourceDocument(tempFirstPagePdf.RemoteWorkFile), new DestinationOptions(DestinationFileFormat.Png)
            {
                PngOptions = new PngDestinationOptions()
                {
                    MaxWidth = "512px",
                    MaxHeight = "512px",
                },
            });

            // Save the single result.
            await thumbnailPngs.Single().RemoteWorkFile.SaveAsync("thumbnail.png");
        }
    }
}
