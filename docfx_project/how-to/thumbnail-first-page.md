# How to Remove the First Page of a Document, Producing a New PDF

This guide explains how to create a PNG thumbnail for the first page of a
document.

PrizmDoc Server does not currently allow you to extract specific pages and
resize to PNG in a single operation. However, you can still accomplish this
efficiently as a simple, two-step process:

1. Extract just the first page to a temporary PDF (which you never need to actually download)
2. Convert that temporary PDF to a thumbnail PNG

Here's how you do that in code.

First, create a [ProcessingContext] for your conversion:

```csharp
var context = client.CreateProcessingContext();
```

Then, extract just the first page to a temporary PDF by calling
[ConvertToPdfAsync], passing in a [SourceDocument] with a `pages` argument set
to `"1"`:

```csharp
var tempFirstPagePdf = await context.ConvertToPdfAsync(new SourceDocument("project-proposal.docx", pages: "1"));
```

This will upload the file to PrizmDoc Server, ask PrizmDoc Server to convert
pages 2 and following to a PDF, and then return once the conversion is complete.

The returned result is just _metadata_ about the output; the actual temporary
PDF file has not been downloaded. And, in this case, we don't even need to
download it. Instead, we'll just use it as input to another operation.

Next, we convert this temporary PDF to a thumbnail PNG by calling
[ConvertAsync]:

```csharp
var thumbnailPngs = await context.ConvertAsync(new SourceDocument(tempFirstPagePdf.RemoteWorkFile), new DestinationOptions(DestinationFileFormat.Png)
{
  PngOptions = new PngDestinationOptions() {
    MaxWidth = "512px",
    MaxHeight = "512px"
  }
});
```

This will start a conversion of the temporary PDF to a PNG which will not be
larger than the specified `maxWidth` and `maxHeight` and then return when the
conversion is finished. The [ConvertPagesToPngsAsync] method always returns a
collection of results, one result for each page that was converted to a PNG. In
our case, there is just one page.

Finally, to actually download the single PNG result, call
`SaveAsync` on the single result:

```csharp
await thumbnailPngs.Single().RemoteWorkFile.SaveAsync("thumbnail.png");
```

Notice an important and powerful concept here: It is possible to pass the output
of one operation as input to a second operation without downloading and
re-uploading the temporary file from and to PrizmDoc Server. You can construct a
[SourceDocument] using _either_ a local file path _or_ an existing
[RemoteWorkFile]. The SDK will automatically upload the files for any
[SourceDocument] instances which use a local file path and automatically reuse
the existing [RemoteWorkFile] for any [SourceDocument] instances constructed for
a [RemoteWorkFile].

Here is a complete example:

```csharp
using System;
using System.IO;
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
      var client = new PrizmDocServerClient(/* your connection info */);

      var context = client.CreateProcessingContext();

      // Extract the first page as an intermediate PDF. We won't ever bother
      // downloading this from PrizmDoc Server.
      var tempFirstPagePdf = await context.ConvertToPdfAsync(new SourceDocument("project-proposal.docx", pages: "1"));

      // Convert the PDF to PNGs, specifying a max width and height. We'll get
      // back a collection of results, one per page. In our case, there is only
      // one page.
      var thumbnailPngs = await context.ConvertAsync(new SourceDocument(tempFirstPagePdf.RemoteWorkFile), new DestinationOptions(DestinationFileFormat.Png)
      {
        PngOptions = new PngDestinationOptions() {
          MaxWidth = "512px",
          MaxHeight = "512px"
        }
      });

      // Save the single result.
      await thumbnailPngs.Single().RemoteWorkFile.SaveAsync("thumbnail.png");
    }
  }
}
```

[RemoteWorkFile]: xref:Accusoft.PrizmDocServer.RemoteWorkFile
[SourceDocument]: xref:Accusoft.PrizmDocServer.Conversion.SourceDocument
[ProcessingContext]: xref:Accusoft.PrizmDocServer.ProcessingContext
[ConvertToPdfAsync]: xref:Accusoft.PrizmDocServer.ProcessingContext.ConvertToPdfAsync(System.String,Accusoft.PrizmDocServer.Conversion.HeaderFooterOptions,Accusoft.PrizmDocServer.Conversion.HeaderFooterOptions)
[ConvertAsync]: xref:Accusoft.PrizmDocServer.ProcessingContext.ConvertAsync(System.String,Accusoft.PrizmDocServer.Conversion.DestinationFileFormat)
