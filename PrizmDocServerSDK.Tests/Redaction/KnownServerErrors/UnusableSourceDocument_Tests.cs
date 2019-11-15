using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Redaction.KnownServerErrors.Tests
{
    [TestClass]
    public class UnusableSourceDocument_Tests
    {
        [TestMethod]
        public async Task CreateRedactionsAsync_fails_with_a_useful_error_message_when_the_source_document_is_unusable()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.CreateRedactionsAsync("documents/corrupted-page-count.pdf", new[] { new RegexRedactionMatchRule("wat") });
                }, "The remote server encountered an error when trying to create redactions for the given document. There may be a problem with the document itself.");
        }
    }
}
