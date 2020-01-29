using System;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer;
using Accusoft.PrizmDocServer.Conversion;

namespace Demos
{
    /// <summary>
    /// Demo program which combines pages from multiple documents to create a new PDF.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            File.Delete("output.pdf");

            var prizmDocServer = new PrizmDocServerClient(Environment.GetEnvironmentVariable("BASE_URL"), Environment.GetEnvironmentVariable("API_KEY"));

            // Take a DOCX file and replace its cover page with a boilerplate cover,
            // append a boilerplate back page, and then produce a new PDF.
            ConversionResult result = await prizmDocServer.CombineToPdfAsync(
                new[]
                {
                    new ConversionSourceDocument("boilerplate-cover-page.pdf"), // start with a boilerplate cover page
                    new ConversionSourceDocument("project-proposal.docx", pages: "2-"), // keep all but the first page of the "main" document
                    new ConversionSourceDocument("boilerplate-back-page.pdf"), // end with a boilerplate back page
                });

            // Save the result to "output.pdf".
            await result.RemoteWorkFile.SaveAsync("output.pdf");
        }
    }
}
