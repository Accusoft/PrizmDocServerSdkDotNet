# PrizmDoc Server .NET SDK **(BETA)**

The PrizmDoc Server .NET SDK is a wrapper around the PrizmDoc Server REST API.
It manages the low-level HTTP details for you, making it easy to consume
PrizmDoc Server functionality in .NET.

PrizmDoc Server is a powerful document processing engine you can use to:

- Convert documents to PDF, TIFF, JPEG, PNG, or SVG
- Combine documents to PDF or TIFF
- Extract pages from documents
- Create thumbnail images for document pages
- Apply headers and footers to documents
- Apply OCR to produce a text-searchable PDF
- Automatically identify text to be redacted by regex
- Redact to PDF or plain text
- Burn-in annotations to PDF
- Identify form fields in PDFs or images
- Search for text in a document

<div class="warning">
<p><b>NOTE:</b> This beta version of the PrizmDoc Server .NET SDK only exposes a
subset of PrizmDoc Server functionality:</p>
<ul>
<li>Convert documents to PDF, TIFF, JPEG, PNG, or SVG</li>
<li>Combine documents to PDF or TIFF</li>
<li>Extract pages from documents</li>
<li>Create thumbnail images for document pages</li>
<li>Apply headers and footers to documents</li>
<li>Apply OCR to produce a text-searchable PDF</li>
<li>Automatically identify text to be redacted by regex</li>
</ul>
<p>We will continue to expose more functionality in future beta releases.
During the beta, the API surface may change from beta release to beta
release.</p>
</div>

## Getting Started

### 1. Get Access to a PrizmDoc Server Deployment

Before you can get started with this SDK, you'll need access to a PrizmDoc
Server deployment. There are two ways to go about that:

1. Sign up for a free [PrizmDoc Cloud] account and use Accusoft's cloud-hosted PrizmDoc Server (easiest to get started).
2. Become a [PrizmDoc Viewer] customer and host PrizmDoc Server yourself.

### 2. Add the PrizmDoc Server .NET SDK NuGet Package

To use the SDK, add the `Accusoft.PrizmDocServerSDK` nuget package to your
project:

```bash
dotnet add package Accusoft.PrizmDocServerSDK --version 1.0.0-beta.*
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

- See the [How To] guides for concrete examples.
- Consult the [API Reference] for detailed information.

## Source Code

The source for this library is [available on GitHub](https://github.com/Accusoft/PrizmDocServerDotNetSDK).

[PrizmDoc Viewer]: https://www.accusoft.com/products/prizmdoc-suite/prizmdoc-viewer
[PrizmDoc Cloud]: https://cloud.accusoft.com
[PrizmDocServerClient]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient
[How To]: how-to/index.md
[API Reference]: xref:Accusoft.PrizmDocServer
