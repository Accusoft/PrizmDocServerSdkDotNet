using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer;
using Accusoft.PrizmDocServer.Conversion;

namespace Demos
{
    /// <summary>
    /// Demo program which creates PNG files for each page of a document.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var prizmDocServer = new PrizmDocServerClient(Environment.GetEnvironmentVariable("BASE_URL"), Environment.GetEnvironmentVariable("API_KEY"));

            // Take a DOCX file and convert each of its pages to a PNG.
            IEnumerable<ConversionResult> results = await prizmDocServer.ConvertAsync("project-proposal.docx", DestinationFileFormat.Png);

            // Save each result to a PNG file.
            for (int i = 0; i < results.Count(); i++)
            {
                await results.ElementAt(i).RemoteWorkFile.SaveAsync($"page-{i + 1}.png");
            }
        }
    }
}
