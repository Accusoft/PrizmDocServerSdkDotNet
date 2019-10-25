# PrizmDoc Server .NET SDK **(BETA)**

.NET wrapper for the PrizmDoc Server REST API, delivered as a NuGet package.

## Installing

Add the `Accusoft.PrizmDocServerSDK` nuget package to your project:

```bash
dotnet add package Accusoft.PrizmDocServerSDK --version 1.0.0-beta.*
```

## Instantiating a [PrizmDocServerClient]

First, construct a [PrizmDocServerClient] instance to specify how to connect to
your PrizmDoc Server backend.

You can connect to either self-hosted PrizmDoc Server or to Accusoft's [PrizmDoc
Cloud].

For self-hosted PrizmDoc Server, just provide the base URL to your PrizmDoc
Server deployment:

```csharp
var prizmDocServer = new PrizmDocServerClient("http://localhost:18681");
```

For PrizmDoc Cloud, provide the base URL and your API key:

```csharp
var prizmDocServer = new PrizmDocServerClient("https://api.accusoft.com", "YOUR_API_KEY");
```

Then, use any of the powerful [PrizmDocServerClient] methods to process your
documents. See the [How To] guides for examples.

[PrizmDoc Cloud]: https://cloud.accusoft.com
[PrizmDocServerClient]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient

## Where to Next?

- See the [How To] guides for concrete examples.
- Consult the [API Reference] for detailed information.

## Source Code

The source for this library is [available on GitHub](https://github.com/Accusoft/PrizmDocServerDotNetSDK).

[How To]: how-to/index.md
[API Reference]: xref:Accusoft.PrizmDocServer
