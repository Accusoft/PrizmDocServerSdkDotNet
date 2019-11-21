using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Burning.KnownServerErrors.Tests
{
    [TestClass]
    public class SourceDocumentDoesNotExist_Tests
    {
        [TestMethod]
        public async Task BurnMarkupAsync_fails_with_a_useful_error_message_when_the_source_document_cannot_be_found()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();
            RemoteWorkFile existingMarkupFile = await affinitySession.UploadAsync("documents/confidential-contacts.pdf.markup.json");
            RemoteWorkFile nonExistentSourceDocument = new RemoteWorkFile(affinitySession, "non-existent-id", existingMarkupFile.AffinityToken, "pdf");

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.BurnMarkupAsync(nonExistentSourceDocument, existingMarkupFile);
                }, "Could not use the given RemoteWorkFile as the source document: the work file resource could not be found on the remote server. It may have expired.");
        }
    }
}
