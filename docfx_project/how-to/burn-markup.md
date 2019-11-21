# How to Burn In Markup, Producing a New PDF

First, create a [PrizmDocServerClient]:

```csharp
var prizmDocServer = new PrizmDocServerClient(/* your connection info */);
```

Then, call [BurnMarkupAsync] providing 1) the original document and 2) the
markup JSON file which defines the areas to be redacted:

```csharp
RemoteWorkFile result = await prizmDocServer.BurnMarkupAsync("original.pdf", "markup.json");
```

This will upload both the original document and markup JSON file to PrizmDoc
Server, ask PrizmDoc Server to burn in the markup, producing a new redacted PDF,
and then return once the burning process is complete.

The returned result is just _metadata_ about the output; the actual output file
has not been downloaded yet. To actually download the result from PrizmDoc
Server, call `SaveAsync` on the returned result:

```csharp
await result.RemoteWorkFile.SaveAsync("redacted.pdf");
```

Or, if you'd prefer instead to download the bytes to a stream, call
`result.RemoteWorkFile.CopyToAsync`:

```csharp
await result.RemoteWorkFile.CopyToAsync(myStream);
```

Here is a complete example:

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer;
using Accusoft.PrizmDocServer.Redaction;

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

            RemoteWorkFile result = await prizmDocServer.BurnMarkupAsync("original.pdf", "markup.json");

            await result.SaveAsync("redacted.pdf");
        }
    }
}
```

There are additional overloads of [BurnMarkupAsync] which allow you to use
existing [RemoteWorkFile] instances instead of local file paths. See the
[PrizmDocServerClient] API reference for more information.

[PrizmDocServerClient]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient
[BurnMarkupAsync]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient.BurnMarkupAsync(System.String,System.String)
[RemoteWorkFile]: xref:Accusoft.PrizmDocServer.RemoteWorkFile
