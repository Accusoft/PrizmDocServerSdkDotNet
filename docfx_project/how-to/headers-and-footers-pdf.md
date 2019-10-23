# How to Apply Headers and Footers

This guide explains how to apply headers and footers to a document, producing a
new PDF.

First, create a [ProcessingContext]:

```csharp
var context = client.CreateProcessingContext();
```

Then, call [ConvertToPdfAsync] to create a PDF, using the `header` and/or
`footer` arguments to define the header and/or footer content you want appended
to each page.

For example, code like this:

```csharp
var result = await context.ConvertToPdfAsync("project-proposal.docx",
  header: new HeaderFooterOptions()
  {
    Color = "#0000FF", // blue
    Lines = new List<HeaderFooterLine>
    {
      new HeaderFooterLine { Left = "North West", Center = "North", Right = "North East" },
      new HeaderFooterLine { Left = "Page {{pageNumber}} of {{pageCount}}" },
    }
  },
  footer: new HeaderFooterOptions()
  {
    Color = "#FF0000", // red
    Lines = new List<HeaderFooterLine>
    {
      new HeaderFooterLine { Center = "BATES{{pageNumber+4000,10}}" },
      new HeaderFooterLine { Left = "South West", Center = "South", Right = "South East" },
    }
  }
);
```

Would append headers and footers to each page like so:

<img class="sample-document-page" src="../images/example-headers-and-footers.png" />

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
using System.Collections.Generic;
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
      File.Delete("output.pdf");

      var client = new PrizmDocServerClient(Environment.GetEnvironmentVariable("BASE_URL"), Environment.GetEnvironmentVariable("API_KEY"));

      var context = client.CreateProcessingContext();

      // Take a DOCX file, append headers and footers to each page (expanding
      // the page size), and convert it to a PDF.
      var result = await context.ConvertToPdfAsync("project-proposal.docx",
        header: new HeaderFooterOptions
        {
          Color = "#0000FF", // blue
          Lines = new List<HeaderFooterLine>
          {
            new HeaderFooterLine { Left = "Top Left", Center = "Top", Right = "Top Right" },
            new HeaderFooterLine { Center = "Page {{pageNumber}} of {{pageCount}}" },
          }
        },
        footer: new HeaderFooterOptions
        {
          Color = "#FF0000", // red
          Lines = new List<HeaderFooterLine>
          {
            new HeaderFooterLine { Center = "BATES{{pageNumber+4000,10}}" },
            new HeaderFooterLine { Left = "Bottom Left", Center = "Bottom", Right = "Bottom Right" },
          }
        }
      );

      // Save the result to "output.pdf".
      await result.RemoteWorkFile.SaveAsync("output.pdf");
    }
  }
}
```

## Dynamic Tokens

The following "tokens" in header or footer text strings will be dynamically replaced:

Token | Dynamic Value
------|--------------
`{{pageCount}}` | Total page count.
`{{pageNumber}}` | Current page number (1-indexed). The first page of the document would use the value `1`.

### Extended Syntax for `{{pageNumber}}` to Support Bates Numbering

The `{{pageNumber}}` token has a special extended syntax to support Bates
numbering.

First, you can easily add an offset to the pageNumber using the syntax
`{{pageNumber+c}}`, where `c` is an integer. For example, `{{pageNumber+50}}`
would use the current page number plus an offset of 50. The first page of the
document would use the value `51`. This is useful if you are producing multiple
documents and want the page numbering to continue across documents.

Second, you can also specify that the page number value be padded with zeros to
always match a minimum character width. You do this by appending `,n`, where `n`
is the minimum number of characters the number must use. For example,
`{{pageNumber+50,8}}` would use the current page number plus an offset of 50,
and left-padded with zeros to ensure the total character width was always at
least 8. The first page of the document would use the value `00000051`.

## Font Size and Family

There are additional properties for specifying the font size and family. See
[HeaderFooterOptions] for more information.

[ProcessingContext]: xref:Accusoft.PrizmDocServer.ProcessingContext
[HeaderFooterOptions]: xref:Accusoft.PrizmDocServer.Conversion.HeaderFooterOptions
[ConvertToPdfAsync]: xref:Accusoft.PrizmDocServer.ProcessingContext.ConvertToPdfAsync(System.String,Accusoft.PrizmDocServer.Conversion.HeaderFooterOptions,Accusoft.PrizmDocServer.Conversion.HeaderFooterOptions)
