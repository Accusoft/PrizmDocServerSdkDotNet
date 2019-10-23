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
var client = new PrizmDocServerClient("http://localhost:18681");
```

For PrizmDoc Cloud, provide the base URL and your API key:

```csharp
var client = new PrizmDocServerClient("https://api.accusoft.com", "YOUR_API_KEY");
```

## Create a [ProcessingContext] Each Time You Need to Do Some Work

Every interaction with PrizmDoc Server involves some sort of processing workflow
where you start with one or more input files, perform one or more operations,
and end up with one or more output files.

With this SDK, any time you need to perform one or more operations, you must
first create a new [ProcessingContext]. Under the hood, we use the
[ProcessingContext] to manage affinity with the particular machine that is
responsible for the remote processing work. We'll spare you the details, but as
a developer using this SDK, there are two important rules you should follow:

  1. **Create a new [ProcessingContext] any time you need to perform distinct
     work on one or more local input files.** For example, if you have a local
     directory of documents and you want to convert them all to PDF, you would
     create a new [ProcessingContext] for _each_ file, use each
     [ProcessingContext] to convert the file to a PDF, and then download the
     result. Using a distinct [ProcessingContext] for each conversion operation
     ensures that the work will be distributed across machines in the PrizmDoc
     Server cluster. Do _not_ use a single [ProcessingContext] to convert all of
     the files in the directory! It will work, but you'll be artificially
     forcing all of the remote conversion work to be done on a single machine in
     the cluster.

  2. The only time you should ever reuse a [ProcessingContext] for multiple
     operations is when you need to pass intermediate output of one operation as
     input to a subsequent operation without actually downloading it locally
     first.

In general, you should think of a
[ProcessingContext] as a cheap, short-lived object that should be used only as
long as you need it. As soon as you have downloaded the output, stop using it.

Creating a [ProcessingContext] is simple:

```csharp
var context = client.CreateProcessingContext();
```

[PrizmDoc Cloud]: https://cloud.accusoft.com
[PrizmDocServerClient]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient
[ProcessingContext]: xref:Accusoft.PrizmDocServer.ProcessingContext

## Where to Next?

- See the [How To] guides for concrete examples.
- Consult the [API Reference] for detailed information.

## Source Code

The source for this library is [available on GitHub](https://github.com/Accusoft/PrizmDocServerDotNetSDK).

[How To]: how-to/index.md
[API Reference]: xref:Accusoft.PrizmDocServer
