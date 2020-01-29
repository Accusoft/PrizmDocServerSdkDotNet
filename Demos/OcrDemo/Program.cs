using System;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer;
using Accusoft.PrizmDocServer.Conversion;

namespace Demos
{
    /// <summary>
    /// Demo program which performs OCR on an image and creates a new text-searchable PDF.
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

            Console.WriteLine("Performing OCR on \"chaucer-scan-3-pages.pdf\"... (this may take a while)");
            ConversionResult result = await prizmDocServer.OcrToPdfAsync("chaucer-scan-3-pages.pdf");

            Console.WriteLine("Saving to \"output.pdf\"...");
            await result.RemoteWorkFile.SaveAsync("output.pdf");

            Console.WriteLine("Done!");
        }
    }
}
