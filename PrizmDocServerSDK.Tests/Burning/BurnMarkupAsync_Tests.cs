using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Burning.Tests
{
    [TestClass]
    public class BurnMarkupAsync_Tests
    {
        [TestMethod]
        public async Task Can_use_local_file_paths_for_both_document_and_markup_JSON()
        {
            // Arrange
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            // Act
            RemoteWorkFile result = await prizmDocServer.BurnMarkupAsync("documents/confidential-contacts.pdf", "documents/confidential-contacts.pdf.markup.json");

            // Assert
            await result.SaveAsync("burned.pdf");
            await this.AssertRedactionOccurredFor(result);
        }

        [TestMethod]
        public async Task Can_use_local_file_path_for_document_and_RemoteWorkFile_for_markup_JSON()
        {
            // Arrange
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();
            RemoteWorkFile markupJson = await affinitySession.UploadAsync("documents/confidential-contacts.pdf.markup.json");

            // Act
            RemoteWorkFile result = await prizmDocServer.BurnMarkupAsync("documents/confidential-contacts.pdf", markupJson);

            // Assert
            await result.SaveAsync("burned.pdf");
            await this.AssertRedactionOccurredFor(result);
        }

        [TestMethod]
        public async Task Can_use_RemoteWorkFile_for_document_and_local_file_path_for_markup_JSON()
        {
            // Arrange
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();
            RemoteWorkFile document = await affinitySession.UploadAsync("documents/confidential-contacts.pdf");

            // Act
            RemoteWorkFile result = await prizmDocServer.BurnMarkupAsync(document, "documents/confidential-contacts.pdf.markup.json");

            // Assert
            await result.SaveAsync("burned.pdf");
            await this.AssertRedactionOccurredFor(result);
        }

        [TestMethod]
        public async Task Can_use_RemoteWorkFile_instances_for_both_document_and_markup_JSON()
        {
            // Arrange
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();
            RemoteWorkFile document = await affinitySession.UploadAsync("documents/confidential-contacts.pdf");
            RemoteWorkFile markupJson = await affinitySession.UploadAsync("documents/confidential-contacts.pdf.markup.json");

            // Act
            RemoteWorkFile result = await prizmDocServer.BurnMarkupAsync(document, markupJson);

            // Assert
            await result.SaveAsync("burned.pdf");
            await this.AssertRedactionOccurredFor(result);
        }

        [MultiServerTestMethod]
        public async Task Can_use_RemoteWorkFile_instances_with_different_affinity()
        {
            // Arrange
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            AffinitySession session1 = Util.RestClient.CreateAffinitySession();
            AffinitySession session2 = Util.RestClient.CreateAffinitySession();

            RemoteWorkFile document = await session1.UploadAsync("documents/confidential-contacts.pdf");
            RemoteWorkFile markupJson = await session2.UploadAsync("documents/confidential-contacts.pdf.markup.json");

            Assert.AreNotEqual(document.AffinityToken, markupJson.AffinityToken);

            // Act
            RemoteWorkFile result = await prizmDocServer.BurnMarkupAsync(document, markupJson);

            // Assert
            await result.SaveAsync("burned.pdf");
            await this.AssertRedactionOccurredFor(result);
        }

        private async Task AssertRedactionOccurredFor(RemoteWorkFile result)
        {
            // Quick sanity check to verify redaction actually occurred
            string[] pagesText = await TextUtil.ExtractPagesText(result);
            Assert.IsTrue(pagesText[0].Contains("Peter Parker"), "Hmm, text content we expected to be in the output document was not present. Did something go wrong?");
            Assert.IsFalse(pagesText[0].Contains("hotshotpete@dailybugle.com"), "Content that was expected to be redacted was not actually redacted!");
            Assert.IsTrue(pagesText[0].Contains("(b)(6)"), "Expected redaction reason was not present!");
        }
    }
}
