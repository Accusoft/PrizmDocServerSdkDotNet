# How to Create Redacted Plain Text

Similar to [creating a redacted PDF](redact-to-pdf.md), you can also use
PrizmDoc Server to create redacted plain text. PrizmDoc Server will extract
plain text from the source document and then apply all of your redactions to the
plain text, replacing all portions of redacted content with the string `<Text
Redacted>` (note that, unlike redacted PDF output, redacted plain text output
does NOT use the redaction reason specified in the markup JSON).

As with [creating redacted PDF output](redact-to-pdf.md), creating redacted
plain text is a two-step process:

1. First, you must create a markup JSON file which _defines_ the redactions
   which should be applied. There are different ways to do this:

   - A person can _visually and manually_ select regions of text or content to
     be redacted using the PrizmDoc Viewer viewer control in a browser. When
     they save their work, it will be persisted as a [markup JSON] file
     containing all of their redaction definitions.

   - An application can _automatically_ create redaction definitions using
     PrizmDoc Server and a set of regular expressions defining the kinds of text
     in a document that should be redacted. The output of this will be a
     [markup JSON] file containing all of the automatically-generated redaction
     definitions. This is the approach we will take in the guide below.

2. Second, you use PrizmDoc Server to _apply_ the redactions in your markup JSON to
   the document to produce redacted plain text.

This guide explains how to 1) automatically generate a [markup JSON] file with
redaction definitions for a given document and a set of regular expressions
defining text patterns in that document that should be redacted and 2) burn
the [markup JSON] into the original document, producing a new redacted plain
text file.

## A Visual Example

### Original Document

For this guide, imagine our original document is a two-page PDF with some
confidential contact information:

<img class="sample-document-page" src="../images/confidential-contacts-page-1.png" />

<img class="sample-document-page" src="../images/confidential-contacts-page-2.png" />

### Example Output

Every occurrence of text which PrizmDoc Server redacts will be replaced with the
hard-coded string `<Text Redacted>` in the output (note that the visual options
for redactions, such as the redaction reason, color, and border options, do not
apply to redacted plain text output).

Imagine we want to redact all Social Security Numbers, email addresses, and the
name "Bruce Wayne" in the original document, producing redacted plain text like
this:

```

Page 1 of 2
Confidential Contact Information

NOTICE: The following information in confidential and intended only for internal use within
the Human Resources department. If you find a printed version of this document outside of the
HR offices, kindly burn it at once.

Name
Peter Parker
Position
Photographer
SSN
<Text Redacted>
Phone
1 (800) 698-4637
Email
<Text Redacted>

Name
Clark Kent
Position
Reporter
SSN
<Text Redacted>
Phone
1-800-552-7678
Email
<Text Redacted>

Name
<Text Redacted>
Position
Chairman and CEO of Wayne Enterprises
SSN
<Text Redacted>
Phone
(800) 574-9469
Email
<Text Redacted>

Name
Bob Parr
Position
Insurance Agent
SSN
<Text Redacted>
Phone
1 (800) 207-7847
Email
<Text Redacted>

Name
Helen Parr
Position
Unknown
SSN
<Text Redacted>
Phone
n/a
Email
<Text Redacted>

Name
Violet Parr
Position
n/a
SSN
<Text Redacted>
Phone
n/a
Email
<Text Redacted>

Name
Dash Parr

Page 2 of 2
Position
n/a
SSN
<Text Redacted>
Phone
n/a
Email
<Text Redacted>

Name
Jack-Jack Parr
Position
n/a
SSN
<Text Redacted>
Phone
n/a
Email
n/a

Name
Harry James Potter
Position
Auror Department Head
SSN
<Text Redacted>
Phone
+44 (0) 1256 302 699
Email
<Text Redacted>





END OF FILE
```

_**NOTE:** The visual order of sections on a page may differ in plain text output
from the original document. This is especially true if the original page uses
multiple columns, boxes, sections, and the like. PrizmDoc Server will extract
the sections of text in the order they are defined internally in the document,
which may differ from the order a person would naturally read them in. In the
example above, notice that the page footer (such "Page 1 of 2"), while visually
at the bottom of each page, is actually listed at the "top" of each "page" of
plain text content. This is simply an artifact of how the original document was
defined internally._

## Step 1: Creating a Markup JSON File Defining What Should Be Redacted

You can use the [CreateRedactionsAsync] method to automatically create a markup
JSON file for a document and a set of regular expression rules defining what
kinds of text should be redacted.

### A Simple Example: Finding the Static Text "John Doe"

First, create a [PrizmDocServerClient]:

```csharp
var prizmDocServer = new PrizmDocServerClient(/* your connection info */);
```

Then, call [CreateRedactionsAsync] providing 1) the source document and 2) a
collection of one or more [RegexRedactionMatchRule] instances which define the
kind of text to search for and, when found, how it should be redacted. The
result of this operation will be a new [RemoteWorkFile] containing the markup
JSON.

Here is a simple example which creates redactions for every occurrence of the
text "John Doe" for a local file `"my-document.docx"`:

```csharp
RemoteWorkFile markupJson = await prizmDocServer.CreateRedactionsAsync("my-document.docx", new[] { new RegexRedactionMatchRule(@"John Doe") });
```

This will upload the file to PrizmDoc Server, ask PrizmDoc Server to create a
new [markup JSON] file containing a redaction definition for every occurrence of
the text `"John Doe"`, and then return once the process is complete.

The returned [RemoteWorkFile] is just _metadata_ about the output [markup JSON]
file; it has not actually been downloaded yet. To download the [markup JSON]
file, call `SaveAsync` on the returned [RemoteWorkFile]:

```csharp
await markupJson.SaveAsync("markup.json");
```

### A More Dynamic Example: Finding Social Security Numbers

Let's say you wanted to create redaction definitions for all occurrences of text
which looked like a Social Security Number. The string you pass in to the
constructor of the [RegexRedactionMatchRule] is actually a regular expression,
so this is easy to achieve, like so:

```csharp
var ssnRule = new RegexRedactionMatchRule(@"\d\d\d-\d\d-\d\d\d\d");
```

You could then use this rule to create the redactions:

```csharp
RemoteWorkFile markupJson = await prizmDocServer.CreateRedactionsAsync("my-document.docx", new[] { ssnRule });
await markupJson.SaveAsync("markup.json");
```

### Using Multiple Rules

Of course, you're not limited to using a single regular expression rule. You can pass in as many different rules as you need, like so:

```csharp
var johnDoeRule = new RegexRedactionMatchRule(@"John Doe");
var ssnRule = new RegexRedactionMatchRule(@"\d\d\d-\d\d-\d\d\d\d");
var emailRule = new RegexRedactionMatchRule(@"\S+@\S+\.\S+");

var rules = new[] { johnDoeRule, ssnRule, emailRule };

RemoteWorkFile markupJson = await prizmDocServer.CreateRedactionsAsync("my-document.docx", rules);
await markupJson.SaveAsync("markup.json");
```

## Step 2: Applying the Redactions to Produce Redacted Plain Text

To apply your redactions and create redacted plain text, simply call
[RedactToPlainTextAsync] providing 1) the original document, 2) the markup JSON
file which defines the areas to be redacted, and 3) the line ending format you
want to use (typically either `"\n"` or `"\r\n"`):

```csharp
RemoteWorkFile result = await prizmDocServer.RedactToPlainTextAsync("original.pdf", markupJson, "\n");
```

This will ask PrizmDoc Server to extract plain text from the original document and replace all redacted areas with `"<Text Redacted>"`.

The returned result is just _metadata_ about the output; the actual redacted
plain text file has not been downloaded yet. To actually download the redacted
plain text file from PrizmDoc Server, call `SaveAsync` on the returned result:

```csharp
await result.RemoteWorkFile.SaveAsync("redacted.txt");
```

Or, if you'd prefer instead to download the bytes to a stream, call
`result.RemoteWorkFile.CopyToAsync`:

```csharp
await result.RemoteWorkFile.CopyToAsync(myStream);
```

## Complete Example

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

            var ssnRule = new RegexRedactionMatchRule(@"\d\d\d-\d\d-\d\d\d\d")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "(b)(6)", // NOTE: This will not be used in plain text output.
                },
            };

            var emailRule = new RegexRedactionMatchRule(@"\S+@\S+\.\S+")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "(b)(6)", // NOTE: This will not be used in plain text output.
                },
            };

            var bruceWayneRule = new RegexRedactionMatchRule(@"Bruce Wayne")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    // NOTE: None of these options will be used in the plain text output.
                    Reason = "(b)(1)",
                    FontColor = "#FDE311",
                    FillColor = "#000080",
                    BorderColor = "#000000",
                    BorderThickness = 2,

                    // This arbitrary data will simply be present in the generated markup JSON.
                    Data = new Dictionary<string, string>
                    {
                        { "arbitrary-key-1", "arbitrary-value-1" },
                        { "arbitrary-key-2", "arbitrary-value-2" },
                    },
                },
            };

            var rules = new[] { ssnRule, emailRule, bruceWayneRule };

            // Automatically create markup JSON using the rules above.
            RemoteWorkFile markupJson = await prizmDocServer.CreateRedactionsAsync("confidential-contacts.pdf", rules);

            // Burn the redactions defined in the markup JSON into the document, producing a new redacted plain text file.
            RemoteWorkFile redactedPlainText = await prizmDocServer.RedactToPlainTextAsync("confidential-contacts.pdf", markupJson, "\n");

            // Download and save the redacted plain text
            await redactedPlainText.SaveAsync("redacted.txt");
        }
    }
}
```

## Markup JSON Specification

For the full markup JSON specification, see
https://help.accusoft.com/PrizmDoc/latest/HTML/webframe.html#markup-json-specification.html.

<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />

[PrizmDocServerClient]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient
[RegexRedactionMatchRule]: xref:Accusoft.PrizmDocServer.Redaction.RegexRedactionMatchRule
[CreateRedactionsAsync]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient.CreateRedactionsAsync(System.String,System.Collections.Generic.IEnumerable{Accusoft.PrizmDocServer.Redaction.RedactionMatchRule})
[RedactionCreationOptions]: xref:Accusoft.PrizmDocServer.Redaction.RedactionCreationOptions
[markup JSON]: #markup-json-specification
[RedactToPlainTextAsync]: xref:Accusoft.PrizmDocServer.PrizmDocServerClient.RedactToPlainTextAsync(System.String,System.String,System.String)
[RemoteWorkFile]: xref:Accusoft.PrizmDocServer.RemoteWorkFile
