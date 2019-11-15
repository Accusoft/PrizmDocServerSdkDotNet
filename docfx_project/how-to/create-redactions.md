# How to Automatically Create Redaction Definitions by Regex

With PrizmDoc Server, redacting content is a two-step process:

- First, you must _define_ the content which should be redacted. There are two
  primary ways to do this:

  1. A person can _visually and manually_ select regions of text or content to
     be redacted using the PrizmDoc Viewer viewer control in a browser. When
     they save their work, it will be persisted as a [markup JSON] file
     containing all of their redaction definitions.

  2. An application can _automatically_ create redaction definitions using
     PrizmDoc Server and a set of regular expressions defining the kinds of text
     in a document which ought to be redacted. The output of this will be a
     [markup JSON] file containing all of the automatically-generated redaction
     definitions. **_This how to guide explains how to do just this part._**

- Second, Once you have a [markup JSON] file which defines what should be
  redacted, you use PrizmDoc Server to _burn in_ this markup into a document,
  producing a new PDF where the content has actually been redacted.

This guide explains how to automatically generate a [markup JSON] file with
redaction definitions for a given document and a set of regular expressions
defining text patterns in that document which ought to be redacted. The result
is a [markup JSON] file containing the redaction definitions.

## A Simple Example: Finding the Static Text "John Doe"

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

## A More Dynamic Example: Finding Social Security Numbers

Let's say you wanted to create redaction definitions for all occurrences of text
which looked like a Social Security Number. The string you pass in to the
constructor of the [RegexRedactionMatchRule] is actually a regular expression,
so this is easy to achieve, like so:

```csharp
var ssnRule = new RegexRedactionMatchRule(@"\d\d\d-\d\d-\d\d\d\d");
```

You could then use this rule to create redactions like so:

```csharp
RemoteWorkFile markupJson = await prizmDocServer.CreateRedactionsAsync("my-document.docx", new[] { ssnRule });
await markupJson.SaveAsync("markup.json");
```

## Using Multiple Rules

Of course, you're not limited to using a single regular expression rule. You can pass in as many different rules as you need, like so:

```csharp
var johnDoeRule = new RegexRedactionMatchRule(@"John Doe");
var ssnRule = new RegexRedactionMatchRule(@"\d\d\d-\d\d-\d\d\d\d");
var emailRule = new RegexRedactionMatchRule(@"\S+@\S+\.\S+");

var rules = new[] { johnDoeRule, ssnRule, emailRule };

RemoteWorkFile markupJson = await prizmDocServer.CreateRedactionsAsync("my-document.docx", rules);
await markupJson.SaveAsync("markup.json");
```

## Customizing Redaction Creation Options

When defining a redaction match rule, you can optionally set the `RedactWith`
property to an instance of [RedactionCreationOptions], allowing you more control
over the appearance of the redactions created by this specific rule.

### Redaction Reason

It is common to display some sort of phrase in the middle of a redaction box
explaining why the content was redacted. We call this the redaction _reason_,
and you can set it like so:

```csharp
var projectXRule = new RegexRedactionMatchRule(@"Project X")
{
    RedactWith = new RedactionCreationOptions()
    {
        Reason = "CONFIDENTIAL",
    },
};
```

Each rule you define can have its own `Reason`. All redaction definitions
created by that rule will use the same reason text. For example, you might use
different legal codes as the redaction reason for different regular expression
rules, like so:

```csharp
var johnDoeRule = new RegexRedactionMatchRule(@"John Doe")
{
    RedactWith = new RedactionCreationOptions()
    {
        Reason = "(b)(1)",
    },
};

var ssnRule = new RegexRedactionMatchRule(@"\d\d\d-\d\d-\d\d\d\d")
{
    RedactWith = new RedactionCreationOptions()
    {
        Reason = "(b)(6)",
    },
};
```

### Redaction Appearance

In addition to the `Reason`, [RedactionCreationOptions] allows you to set other
properties, such as `FontColor`, `FillColor`, `BorderColor`, and
`BorderThickness`. Here is an example:

```csharp
var bruceWayneRule = new RegexRedactionMatchRule(@"Bruce Wayne")
{
    RedactWith = new RedactionCreationOptions()
    {
        Reason = "(b)(1)",
        FontColor = "#FDE311", // Use "batman yellow" color for the reason text.
        FillColor = "#080808", // Use a near-black background color.
        BorderColor = "#000000", // Use pure black border color.
        BorderThickness = 2 // Make the border 2-pixels thick.
    },
};
```

See the [RedactionCreationOptions] class for more information.

### Attaching Arbitrary Data

Finally, you can use the `Data` property of [RedactionCreationOptions] to define
your own set of key/value string pairs which will be attached to every redaction
definition in the output [markup JSON]. For example, if you define a rule like
this:

```csharp
var johnDoeRule = new RegexRedactionMatchRule(@"John Doe")
{
    RedactWith = new RedactionCreationOptions()
    {
        Data = new Dictionary<string, string>
        {
            { "user-id", "jdoe" },
            { "age", "32" },
        },
    },
};
```

Then, when inspecting the output [markup JSON], you would find that every
redaction created by this rule would contain a `data` property with the given
`user-id` and `age`.

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
                    Reason = "(b)(6)",
                    Data = new Dictionary<string, string>
                    {
                        { "Generated By", "Acme Redactor Application" },
                    },
                },
            };

            var emailRule = new RegexRedactionMatchRule(@"\S+@\S+\.\S+")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "(b)(6)",
                    Data = new Dictionary<string, string>
                    {
                        { "Generated By", "Acme Redactor Application" },
                    },
                },
            };

            var bruceWayneRule = new RegexRedactionMatchRule(@"Bruce Wayne")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "(b)(1)",
                    FontColor = "#FDE311",
                    FillColor = "#080808",
                    BorderColor = "#000000",
                    BorderThickness = 1,
                    Data = new Dictionary<string, string>
                    {
                        { "Generated By", "Acme Redactor Application" },
                        { "alias", "The Dark Knight" },
                        { "isBatman", "true" },
                    },
                },
            };

            var rules = new[] { ssnRule, emailRule, bruceWayneRule };

            RemoteWorkFile markupJson = await prizmDocServer.CreateRedactionsAsync("confidential-contacts.pdf", rules);
            await markupJson.SaveAsync("markup.json");
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
