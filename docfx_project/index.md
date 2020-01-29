# PrizmDoc Server .NET SDK

Part of the larger [PrizmDoc Viewer] product, PrizmDoc Server is a powerful
document processing engine which hosts a set of low-level document processing
REST APIs. While you could use these REST APIs directly, it requires quite a bit
of effort. This library, the **PrizmDoc Server .NET SDK**, is a wrapper around
the PrizmDoc Server REST APIs, making it easy to use PrizmDoc Server
functionality in .NET.

You can use this library with any deployment of PrizmDoc Server, whether it's
your own self-hosted deployment or Accusoft's cloud-hosted offering. Simply
construct an instance of [PrizmDocServerClient] with the information about how
to connect to PrizmDoc Server and start using any of the document-processing
methods to do things like:

- Convert documents to PDF, TIFF, JPEG, PNG, or SVG
- Combine documents to PDF or TIFF
- Extract pages from documents
- Split and merge pages from various documents
- Create thumbnail images for document pages
- Apply headers and footers to documents
- Perform OCR to produce a text-searchable PDF
- Automatically identify text to be redacted by regex
- Redact to PDF or plain text
- Burn-in annotations to PDF

See the [How To Guides] for more information.

## Getting Started

### 1. Get Access to a PrizmDoc Server Deployment

Before you can get started with this SDK, you'll need access to a PrizmDoc
Server deployment. There are two ways to do that:

1. Sign up for a free [PrizmDoc Cloud] account and use Accusoft's cloud-hosted PrizmDoc Server (easiest way to get started).
2. Become a [PrizmDoc Viewer] customer and host PrizmDoc Server yourself.

### 2. Add the PrizmDoc Server .NET SDK NuGet Package

To use the SDK, add the `Accusoft.PrizmDocServerSDK` NuGet package to your
project:

```bash
dotnet add package Accusoft.PrizmDocServerSDK
```

### 3. Construct a [PrizmDocServerClient]

Construct a new [PrizmDocServerClient], specifying how to connect to your
PrizmDoc Server.

If you're using PrizmDoc Cloud, just provide the base URL and your API key:

```csharp
var prizmDocServer = new PrizmDocServerClient("https://api.accusoft.com", "YOUR_API_KEY");
```

If you're using your own, self-hosted PrizmDoc Server, just provide the base URL:

```csharp
var prizmDocServer = new PrizmDocServerClient("http://localhost:18681");
```

### 4. Start Processing Some Documents!

- See the [How To Guides] for concrete examples.
- Consult the [API Reference] for detailed information.

## Limitations

This SDK is only designed to do server-side document processing with PrizmDoc
Server. It is not intended to support document viewing (for viewing, see the
["Getting Started with Document
Viewing"](https://help.accusoft.com/PrizmDoc/latest/HTML/webframe.html#getting-started-with-document-viewing.html)
guide in the PrizmDoc Viewer product documentation).

There are some PrizmDoc Server REST APIs which this SDK does not yet expose:

- [Search Contexts](https://help.accusoft.com/PrizmDoc/latest/HTML/webframe.html#search-contexts.html) and [Search Tasks](https://help.accusoft.com/PrizmDoc/latest/HTML/webframe.html#search-tasks.html)
- [Form Extractors](https://help.accusoft.com/PrizmDoc/latest/HTML/webframe.html#form-extractors.html)

Additionally, this SDK is not designed to be used to administer PrizmDoc Server instances ([server health](https://help.accusoft.com/PrizmDoc/latest/HTML/webframe.html#health-status.html) and [cluster management](https://help.accusoft.com/PrizmDoc/latest/HTML/webframe.html#cluster-management.html)).

## Source Code

The source for this library is [available on GitHub](https://github.com/Accusoft/PrizmDocServerDotNetSDK).

[PrizmDoc Viewer]: https://www.accusoft.com/products/prizmdoc-suite/prizmdoc-viewer
[PrizmDoc Cloud]: https://cloud.accusoft.com
[PrizmDocServerClient]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient
[How To Guides]: how-to/index.md
[API Reference]: xref:Accusoft.PrizmDocServer
