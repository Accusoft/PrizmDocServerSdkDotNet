using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.PlainTextRedaction.KnownServerErrors.Tests
{
    [TestClass]
    public class SourceDocumentUnusable_Tests
    {
        [TestMethod]
        public async Task RedactToPlainTextAsync_fails_with_a_useful_error_message_when_the_source_document_is_unusable()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.RedactToPlainTextAsync("documents/corrupted-page-count.pdf", "documents/confidential-contacts.pdf.markup.json", "\n");
                }, "The remote server was unable to burn the markup file into the document. It is possible there is a problem with the document itself.");
        }
    }
}
