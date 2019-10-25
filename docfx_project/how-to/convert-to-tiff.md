# How to Convert a File to TIFF

This guide explains how to perform a simple conversion of a file to TIFF.

First, create a [PrizmDocServerClient]:

```csharp
var prizmDocServer = new PrizmDocServerClient(/* your connection info */);
```

Then, call [ConvertAsync] to take a local file, such as
`"project-proposal.docx"`, and have PrizmDoc Server convert it to a TIFF:

```csharp
var results = await prizmDocServer.ConvertAsync("project-proposal.docx", DestinationFileFormat.Tiff);
```

This will upload the file to PrizmDoc Server, ask PrizmDoc Server to convert it
to a TIFF, and then return once the conversion is complete.

The returned results are just _metadata_ about the output; the actual output
file(s) have not been downloaded yet. In this case, since the destination format
supports multiple pages, there will only be a single output file. To actually
download the output TIFF from PrizmDoc Server, call `SaveAsync` on the single
result:

```csharp
await results.Single().RemoteWorkFile.SaveAsync("output.pdf");
```

Or, if you'd prefer instead to download the bytes to a stream, call
`CopyToAsync`:

```csharp
await results.Single().RemoteWorkFile.CopyToAsync(myStream);
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

      // Take a DOCX file and convert it to a TIFF.
      var results = await prizmDocServer.ConvertAsync("project-proposal.docx", DestinationFileFormat.Tiff);

      // Save the result to "output.pdf".
      await results.Single().RemoteWorkFile.SaveAsync("output.pdf");
    }
  }
}
```

There are additional overloads of [ConvertAsync] which provide more
flexibility. See the [PrizmDocServerClient] API reference for more information.

[PrizmDocServerClient]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient
[ConvertAsync]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient.ConvertAsync(System.String,Accusoft.PrizmDocServer.Conversion.DestinationFileFormat)
