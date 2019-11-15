using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Redaction.KnownServerErrors.Tests
{
    [TestClass]
    public class WorkFile_Invalid_Tests
    {
        [TestMethod]
        public async Task When_work_file_does_not_exist()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();
            RemoteWorkFile validWorkFile = await affinitySession.UploadAsync("documents/confidential-contacts.pdf");
            RemoteWorkFile invalidWorkFile = new RemoteWorkFile(affinitySession, "non-existent-id", validWorkFile.AffinityToken, "pdf");

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.CreateRedactionsAsync(invalidWorkFile, new[] { new RegexRedactionMatchRule("dummy rule") });
                }, "Could not use the given RemoteWorkFile as the source document: the work file resource could not be found on the remote server. It may have expired.");
        }
    }
}
