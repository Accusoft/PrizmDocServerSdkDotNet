using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.PlainTextRedaction.KnownServerErrors.Tests
{
    [TestClass]
    public class InvalidLineEndingValue_Tests
    {
        [TestMethod]
        public async Task RedactToPlainTextAsync_fails_with_a_useful_error_message_when_an_unsupported_line_ending_is_used()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.RedactToPlainTextAsync("documents/confidential-contacts.pdf", "documents/confidential-contacts.pdf.markup.json", "wat");
                }, "Unsupported line ending \"wat\". The remote server only supports the following values: \"\\n\", \"\\r\\n\".");
        }
    }
}
