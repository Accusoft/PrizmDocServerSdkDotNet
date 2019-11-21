using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer;
using Accusoft.PrizmDocServer.Redaction;

namespace Demos
{
    /// <summary>
    /// Demo program which automatically generates a set of redaction
    /// definitions for a document based on a set of regular expressions then
    /// burns them in, producing a new, redacted PDF.
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
            File.Delete("redacted.pdf");

            var prizmDocServer = new PrizmDocServerClient(Environment.GetEnvironmentVariable("BASE_URL"), Environment.GetEnvironmentVariable("API_KEY"));

            // -----------------------------------------------------------------
            // Step 1: Create markup JSON containing definitions of the areas we
            //         want to redact.
            // -----------------------------------------------------------------

            // Define a rule which will create a redaction for any text in a
            // document which looks like a social security number (###-##-####),
            // and use the text "(b)(6)" in the center of the redaction
            // rectangle as the reason for redaction.
            var ssnRule = new RegexRedactionMatchRule(@"\d\d\d-\d\d-\d\d\d\d")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "(b)(6)",
                },
            };

            // Define a rule which will create a redaction for any text in a
            // document which looks like an email address (this is a very basic
            // regex, matching things like johndoe@somecompany.com) and use the
            // text "(b)(6)" in the center of the redaction rectangle as
            // the reason for redaction.
            var emailRule = new RegexRedactionMatchRule(@"\S+@\S+\.\S+")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "(b)(6)",
                },
            };

            // Define a rule which will create a redaction for all occurrences
            // of "Bruce Wayne" in a document, use the text "(b)(1)" in the
            // center of the redaction rectangle as the reason for redaction,
            // customize various colors used, and attach some arbitrary
            // key/value string data to all redaction definitions which are
            // created. This arbitrary data will be present in the output markup
            // JSON file.
            var bruceWayneRule = new RegexRedactionMatchRule(@"Bruce Wayne")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "(b)(1)",
                    FontColor = "#FDE311",
                    FillColor = "#000080",
                    BorderColor = "#000000",
                    BorderThickness = 2,
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
            // definitions can later be burned into the document.
            RemoteWorkFile markupJson = await prizmDocServer.CreateRedactionsAsync("confidential-contacts.pdf", rules);

            // -----------------------------------------------------------------
            // Step 2: Burn the markup JSON into the original document,
            //         producing a new, redacted PDF.
            // -----------------------------------------------------------------
            RemoteWorkFile redactedPdf = await prizmDocServer.BurnMarkupAsync("confidential-contacts.pdf", markupJson);

            // Save the result to "redacted.pdf"
            await redactedPdf.SaveAsync("redacted.pdf");
        }
    }
}
