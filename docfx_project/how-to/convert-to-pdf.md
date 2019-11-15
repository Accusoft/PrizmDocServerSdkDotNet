# How to Convert a File to PDF

This guide explains how to perform a simple conversion of a file to PDF.

First, create a [PrizmDocServerClient]:

```csharp
var prizmDocServer = new PrizmDocServerClient(/* your connection info */);
```

Then, call [ConvertToPdfAsync] to take a local file, such as
`"project-proposal.docx"`, and have PrizmDoc Server convert it to a PDF:

```csharp
ConversionResult result = await prizmDocServer.ConvertToPdfAsync("project-proposal.docx");
```

This will upload the file to PrizmDoc Server, ask PrizmDoc Server to convert it
to a PDF, and then return once the conversion is complete.

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

            // Take a DOCX file and convert it to a PDF.
            ConversionResult result = await prizmDocServer.ConvertToPdfAsync("project-proposal.docx");

            // Save the result to "output.pdf".
            await result.RemoteWorkFile.SaveAsync("output.pdf");
        }
    }
}
```

Note that the [ConvertToPdfAsync] methods are actually just convenience wrappers
around the lower-level [ConvertAsync] methods. You could achieve the same sort
of thing with a [ConvertAsync] call like so:

```csharp
IEnumerable<ConversionResult> results = await prizmDocServer.ConvertAsync("project-proposal.docx", DestinationFileFormat.Pdf);
ConversionResult result = results.Single();
```

There are additional overloads for [ConvertToPdfAsync] and [ConvertAsync] which
provide more flexibility. See the [PrizmDocServerClient] API reference for more
information.

[PrizmDocServerClient]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient
[ConvertToPdfAsync]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient.ConvertToPdfAsync(System.String,Accusoft.PrizmDocServer.Conversion.HeaderFooterOptions,Accusoft.PrizmDocServer.Conversion.HeaderFooterOptions)
[ConvertAsync]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient.ConvertAsync(System.Collections.Generic.IEnumerable{Accusoft.PrizmDocServer.Conversion.ConversionSourceDocument},Accusoft.PrizmDocServer.Conversion.DestinationOptions)
