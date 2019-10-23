# How to Remove the First Page of a Document, Producing a New PDF

This guide explains how to remove the first page of a document, producing a new
PDF.

First, create a [ProcessingContext] for your conversion:

```csharp
var context = client.CreateProcessingContext();
```

Then, call [ConvertToPdfAsync], passing in a [SourceDocument] with a `pages`
argument set to `"2-"`, indicating that you only want pages 2 and following:

```csharp
var result = await context.ConvertToPdfAsync(new SourceDocument("project-proposal.docx", pages: "2-"));
```

This will upload the file to PrizmDoc Server, ask PrizmDoc Server to convert
pages 2 and following to a PDF, and then return once the conversion is complete.

The returned result is just _metadata_ about the output; the actual output file
has not been downloaded yet. To actually download the result from PrizmDoc
Server, call `result.RemoteWorkFile.SaveAsync`:

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

      // Take a DOCX file and convert all of its pages except the first one to a PDF.
      var result = await context.ConvertToPdfAsync(new SourceDocument("project-proposal.docx", pages: "2-"));

      // Save the result to "output.pdf".
      await result.RemoteWorkFile.SaveAsync("output.pdf");
    }
  }
}
```

The optional `pages` argument for a [SourceDocument] allows you to do a lot more
than simply remove the first page. Just like a "pages" text box in a print
dialog, the value can be a single page like `"2"`, a comma-delimited list of
specific pages like `"1, 4, 5"`, an open-ended page range like `"2-"` (page 2
through the end of the document), or a combination of these, like `"2, 4-9, 12-"`.

[SourceDocument]: xref:Accusoft.PrizmDocServer.Conversion.SourceDocument
[ProcessingContext]: xref:Accusoft.PrizmDocServer.ProcessingContext
[ConvertToPdfAsync]: xref:Accusoft.PrizmDocServer.ProcessingContext.ConvertToPdfAsync(System.String,Accusoft.PrizmDocServer.Conversion.HeaderFooterOptions,Accusoft.PrizmDocServer.Conversion.HeaderFooterOptions)
