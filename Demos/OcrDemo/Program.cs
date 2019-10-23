using System;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer;

namespace Demos
{
  class Program
  {
    static void Main(string[] args)
    {
      MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync()
    {
      File.Delete("output.pdf");

      var client = new PrizmDocServerClient(Environment.GetEnvironmentVariable("BASE_URL"), Environment.GetEnvironmentVariable("API_KEY"));

      var context = client.CreateProcessingContext();

      Console.WriteLine("Performing OCR on \"chaucer-scan-3-pages.pdf\"... (this may take a while)");
      var result = await context.OcrToPdfAsync("chaucer-scan-3-pages.pdf");

      Console.WriteLine("Saving to \"output.pdf\"...");
      await result.RemoteWorkFile.SaveAsync("output.pdf");

      Console.WriteLine("Done!");
    }
  }
}
