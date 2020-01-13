using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer;
using Accusoft.PrizmDocServer.Redaction;

namespace Demos
{
    /// <summary>
    /// Demo program which 1) automatically generates a set of redaction
    /// definitions for a document based on a set of regular expressions and
    /// then 2) uses these redaction definitions to produce a redacted plain
    /// text form of the source document.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            // Delete any existing output file before we get started.
            File.Delete("redacted.txt");

            var prizmDocServer = new PrizmDocServerClient(Environment.GetEnvironmentVariable("BASE_URL"), Environment.GetEnvironmentVariable("API_KEY"));

            // -----------------------------------------------------------------
            // Step 1: Create markup JSON containing definitions of the areas we
            //         want to redact.
            // -----------------------------------------------------------------

            // Define a rule which will create a redaction for any text in a
            // document which looks like a social security number (###-##-####),
            // and, for PDF output, use the text "(b)(6)" in the center of the
            // redaction rectangle as the reason for redaction. NOTE: For plain
            // text output, the redaction reason will NOT be used. Instead, the
            // plain text will simply show "<Text Redacted>" for any occurrences
            // replaced.
            var ssnRule = new RegexRedactionMatchRule(@"\d\d\d-\d\d-\d\d\d\d")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "(b)(6)", // NOTE: This will not be used in plain text output.
                },
            };

            // Define a rule which will create a redaction for any text in a
            // document which looks like an email address (this is a very basic
            // regex, matching things like johndoe@somecompany.com) and, for PDF
            // output, use the text "(b)(6)" in the center of the redaction
            // rectangle as the reason for redaction. NOTE: For plain text
            // output, the redaction reason will NOT be used. Instead, the plain
            // text will simply show "<Text Redacted>" for any occurrences
            // replaced.
            var emailRule = new RegexRedactionMatchRule(@"\S+@\S+\.\S+")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "(b)(6)", // NOTE: This will not be used in plain text output.
                },
            };

            // Define a rule which will create a redaction for all occurrences
            // of "Bruce Wayne" in a document and, for PDF output, use the text
            // "(b)(1)" in the center of the redaction rectangle as the reason
            // for redaction, customize various colors used, and attach some
            // arbitrary key/value string data to all redaction definitions
            // which are created. This arbitrary data will be present in the
            // output markup JSON file. NOTE: For plain text output, the
            // redaction reason and custom styling options will NOT be used.
            // Instead, the output plain text will simply show "<Text Redacted>"
            // for any occurrences replaced.
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

            // Automatically create a markup.json file with redaction
            // definitions based upon regular expression rules for a given
            // document. Any text in the document which matches one of the regex
            // rules will have a redaction definition created for that portion
            // of the document. The output markup.json file with its redaction
            // definitions can later be applied to the document.
            RemoteWorkFile markupJson = await prizmDocServer.CreateRedactionsAsync("confidential-contacts.pdf", rules);

            // -----------------------------------------------------------------
            // Step 2: Apply the markup JSON to the original document,
            //         producing a new, redacted plain text file.
            // -----------------------------------------------------------------
            RemoteWorkFile redactedPlainText = await prizmDocServer.RedactToPlainTextAsync("confidential-contacts.pdf", markupJson, "\n");

            // Save the result to "redacted.txt"
            await redactedPlainText.SaveAsync("redacted.txt");
        }
    }
}
