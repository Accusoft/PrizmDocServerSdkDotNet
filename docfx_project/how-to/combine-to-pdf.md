# How to Combine Pages from Multiple Documents Into a Single New PDF

This guide explains how to combine pages from multiple documents into a single new PDF.

First, create a [ProcessingContext] for your conversion:

```csharp
var context = client.CreateProcessingContext();
```

Then, call [CombineToPdfAsync] with a collection of [SourceDocument] instances, resulting in PrizmDoc Server combining the requested pages of those documents into a single new PDF:

```csharp
var result = await context.CombineToPdfAsync(
  new[] {
    new SourceDocument("boilerplate-cover-page.pdf"), // start with a boilerplate cover page
    new SourceDocument("project-proposal.docx", pages: "2-"), // keep all but the first page of the "main" document
    new SourceDocument("boilerplate-back-page.pdf") // end with a boilerplate back page
  }
);
```

This will upload _all_ of the local files specified for each [SourceDocument] to
PrizmDoc Server, ask PrizmDoc Server to combine the specified documents and
pages to a PDF, and then return once the conversion is complete.

The returned result is just _metadata_ about the output; the actual output file
has not been downloaded yet. To actually download the result from PrizmDoc
Server, call `SaveAsync` on the returned result:

```csharp
await result.RemoteWorkFile.SaveAsync("output.pdf");
```

Or, if you'd prefer instead to download the bytes to a stream, call
`result.RemoteWorkFile.CopyToAsync`:

```csharp
await result.RemoteWorkFile.CopyToAsync(myStream);
```

Here is a complete example:

```csharp
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
      var client = new PrizmDocServerClient(/* your connection info */);

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
```

The [CombineToPdfAsync] method has additional options for applying headers and
footers. See the [CombineToPdfAsync] reference documentation for more
information. For an example of how to specify header and footer information, see
[How to Apply Headers and Footers](headers-and-footers-pdf.md).

Also, note that the [CombineToPdfAsync] methods are actually just convenience
wrappers around the lower-level [ConvertAsync] methods. You could achieve the
same sort of thing with a [ConvertAsync] call like so:

```csharp
var results = await context.ConvertAsync("project-proposal.docx", DestinationFileFormat.Pdf);
var result = results.Single();
```

See the [ProcessingContext] API reference for more information.

[ProcessingContext]: xref:Accusoft.PrizmDocServer.ProcessingContext
[SourceDocument]: xref:Accusoft.PrizmDocServer.Conversion.SourceDocument
[CombineToPdfAsync]: xref:Accusoft.PrizmDocServer.ProcessingContext.CombineToPdfAsync(System.Collections.Generic.IEnumerable{Accusoft.PrizmDocServer.Conversion.SourceDocument},Accusoft.PrizmDocServer.Conversion.HeaderFooterOptions,Accusoft.PrizmDocServer.Conversion.HeaderFooterOptions)
[HeaderFooterOptions]: xref:Accusoft.PrizmDocServer.Conversion.HeaderFooterOptions
[ConvertAsync]: xref:Accusoft.PrizmDocServer.ProcessingContext.ConvertAsync(System.Collections.Generic.IEnumerable{Accusoft.PrizmDocServer.Conversion.SourceDocument},Accusoft.PrizmDocServer.Conversion.DestinationOptions)
