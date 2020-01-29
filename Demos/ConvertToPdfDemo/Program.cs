using System;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer;
using Accusoft.PrizmDocServer.Conversion;

namespace Demos
{
    /// <summary>
    /// Demo program which converts a file to a PDF.
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

            // Take a DOCX file and convert it to a PDF.
            ConversionResult result = await prizmDocServer.ConvertToPdfAsync("project-proposal.docx");

            // Save the result to "output.pdf".
            await result.RemoteWorkFile.SaveAsync("output.pdf");
        }
    }
}
