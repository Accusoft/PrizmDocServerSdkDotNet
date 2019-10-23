using System;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer;
using Accusoft.PrizmDocServer.Conversion;

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

      // Take a DOCX file and replace its cover page with a boilerplate cover,
      // append a boilerplate back page, and then produce a new PDF.
      var result = await context.CombineToPdfAsync(
        new[] {
          new SourceDocument("boilerplate-cover-page.pdf"), // start with a boilerplate cover page
          new SourceDocument("project-proposal.docx", pages: "2-"), // keep all but the first page of the "main" document
          new SourceDocument("boilerplate-back-page.pdf") // end with a boilerplate back page
        }
      );

      // Save the result to "output.pdf".
      await result.RemoteWorkFile.SaveAsync("output.pdf");
    }
  }
}
