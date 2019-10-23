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

      // Take a DOCX file and convert it to a PDF.
      var result = await context.ConvertToPdfAsync("project-proposal.docx");

      // Save the result to "output.pdf".
      await result.RemoteWorkFile.SaveAsync("output.pdf");
    }
  }
}
