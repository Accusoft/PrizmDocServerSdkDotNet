using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.PlainTextRedaction.KnownServerErrors.Tests
{
    [TestClass]
    public class MarkupFileNotJson_Tests
    {
        [TestMethod]
        public async Task RedactToPlainTextAsync_fails_with_a_useful_error_message_when_the_markup_json_file_is_not_actually_JSON()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.RedactToPlainTextAsync("documents/confidential-contacts.pdf", "documents/example.docx", "\n");
                }, "The remove server was unable to burn the markup file into the document because the markup file was not valid JSON.");
        }
    }
}
