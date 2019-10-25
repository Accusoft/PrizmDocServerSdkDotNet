using System;
using System.Linq;
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
      var prizmDocServer = new PrizmDocServerClient(Environment.GetEnvironmentVariable("BASE_URL"), Environment.GetEnvironmentVariable("API_KEY"));

      // Take a DOCX file and convert each of its pages to a PNG.
      var results = await prizmDocServer.ConvertAsync("project-proposal.docx", DestinationFileFormat.Png);

      // Save each result to a PNG file.
      for (var i=0; i < results.Count(); i++)
      {
        await results.ElementAt(i).RemoteWorkFile.SaveAsync($"page-{i+1}.png");
      }
    }
  }
}
