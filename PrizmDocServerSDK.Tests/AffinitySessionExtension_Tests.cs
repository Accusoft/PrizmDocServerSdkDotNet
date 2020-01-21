using System;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Tests
{
    [TestClass]
    public class AffinitySessionExtension_Tests
    {
        [TestMethod]
        public async Task UploadAsync_throws_if_affinitySession_is_null()
        {
            await UtilAssert.ThrowsExceptionWithMessageContainingAsync<ArgumentNullException>(
                async () => { await AffinitySessionExtensions.UploadAsync(null, "documents/example.docx"); },
                "affinitySession");
        }

        [TestMethod]
        public async Task UploadAsync_throws_if_the_local_file_cannot_be_found()
        {
            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();
            await UtilAssert.ThrowsExceptionWithMessageAsync<FileNotFoundException>(
                async () => { await affinitySession.UploadAsync("documents/does not exist.pdf"); },
                "File not found: \"documents/does not exist.pdf\"");
        }
    }
}
