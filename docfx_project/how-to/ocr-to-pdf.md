# How to OCR Images and PDF files to a Text-Searchable PDF

This guide explains how to perform OCR on one or more images and/or PDF files to
produce a new, text-searchable PDF.

First, create a [PrizmDocServerClient]:

```csharp
var prizmDocServer = new PrizmDocServerClient(/* your connection info */);
```

Then, call [OcrToPdfAsync] to take one or more local files and have PrizmDoc Server
OCR it and produce a text-searchable PDF.

You can use both images (JPEG, PNG, TIFF, BMP, and more) as well as PDFs with
image-only page data. The output will always be a single PDF which is
text-searchable.

For example, you can use a single image as input:

```csharp
var result = await prizmDocServer.OcrToPdfAsync("scan.jpeg");
```

You can also use multiple images as input:

```csharp
var result = await prizmDocServer.OcrToPdfAsync(new SourceDocument[]
{
  new SourceDocument("page-1-scan.jpeg"),
  new SourceDocument("page-2-scan.jpeg"),
  new SourceDocument("page-3-scan.jpeg")
});
```

Or you can use a multi-page PDF as input:

```csharp
var result = await prizmDocServer.OcrToPdfAsync("scanned.pdf");
```

You can even combine these, optionally specifying the specific pages to use for
a particular file:

```csharp
var result = await prizmDocServer.OcrToPdfAsync(new SourceDocument[]
{
  new SourceDocument("boilerplate-cover-page.png"),
  new SourceDocument("contract.pdf", pages: "2-5"),
  new SourceDocument("affidavit.tiff"),
  new SourceDocument("meeting-minutes.jpeg"),
});
```

Whether you use one input or many, the call to [OcrToPdfAsync] will upload the input
files to PrizmDoc Server, ask PrizmDoc Server to perform OCR on these inputs and
produce a single, text-searchable PDF as output which contains all of the input
pages in order.

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
      var prizmDocServer = new PrizmDocServerClient(/* your connection info */);

      // OCR an image-only PDF, creating a new PDF:
      var result = await prizmDocServer.OcrToPdfAsync("scanned.pdf");
      await result.RemoteWorkFile.SaveAsync("output.pdf");

      // OCR a collection of JPEG scans, creating a single output PDF:
      var result = await prizmDocServer.OcrToPdfAsync(new SourceDocument[]
      {
        "scan-page-1.jpg",
        "scan-page-2.jpg",
        "scan-page-3.jpg"
      });
      await result.RemoteWorkFile.SaveAsync("output.pdf");
    }
  }
}
```

There are additional overloads of [OcrToPdfAsync] which offer more options.

Also, note that the [OcrToPdfAsync] methods are actually just convenience
wrappers around the lower-level [ConvertAsync] methods. You could achieve the
same sort of thing with a [ConvertAsync] call like so:

```csharp
var results = await prizmDocServer.ConvertAsync("project-proposal.docx", new DestinationOptions(DestinationFileFormat.Pdf)
{
  PdfOptions = new PdfDestinationOptions
  {
    Ocr = new OcrOptions()
    {
      Language = "english"
    }
  }
});
var result = results.Single();
```

See the [PrizmDocServerClient] API reference for more information.

[PrizmDocServerClient]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient
[Conversion.Result]: xref:Accusoft.PrizmDocServer.Conversion.Result
[Result]: xref:Accusoft.PrizmDocServer.Conversion.Result
[OcrToPdfAsync]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient.OcrToPdfAsync(System.String)
[ConvertAsync]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient.ConvertAsync(System.Collections.Generic.IEnumerable{Accusoft.PrizmDocServer.Conversion.SourceDocument},Accusoft.PrizmDocServer.Conversion.DestinationOptions)
