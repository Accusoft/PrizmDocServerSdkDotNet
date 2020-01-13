using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.PlainTextRedaction.KnownServerErrors.Tests
{
    [TestClass]
    public class MarkupFileDoesNotExist_Tests
    {
        [TestMethod]
        public async Task RedactToPlainTextAsync_fails_with_a_useful_error_message_when_the_markup_file_cannot_be_found()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();
            RemoteWorkFile existingSourceDocument = await affinitySession.UploadAsync("documents/confidential-contacts.pdf");
            RemoteWorkFile nonExistentMarkupFile = new RemoteWorkFile(affinitySession, "non-existent-id", existingSourceDocument.AffinityToken, "pdf");

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.RedactToPlainTextAsync(existingSourceDocument, nonExistentMarkupFile, "\n");
                }, "Could not use the given RemoteWorkFile as the markup JSON file: the work file resource could not be found on the remote server. It may have expired.");
        }
    }
}
