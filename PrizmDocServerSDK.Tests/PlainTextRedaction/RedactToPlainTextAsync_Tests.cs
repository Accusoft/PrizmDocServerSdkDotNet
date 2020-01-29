using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.PlainTextRedaction.Tests
{
    [TestClass]
    public class RedactToPlainTextAsync_Tests
    {
        [TestMethod]
        public async Task Can_use_local_file_paths_for_both_document_and_markup_JSON()
        {
            // Arrange
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            // Act
            RemoteWorkFile result = await prizmDocServer.RedactToPlainTextAsync("documents/confidential-contacts.pdf", "documents/confidential-contacts.pdf.markup.json", "\n");

            // Assert
            await this.AssertPlainTextRedactionOccurredFor(result, "\n");
        }

        [TestMethod]
        public async Task Can_use_single_newline_for_line_endings()
        {
            // Arrange
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            // Act
            RemoteWorkFile result = await prizmDocServer.RedactToPlainTextAsync("documents/confidential-contacts.pdf", "documents/confidential-contacts.pdf.markup.json", "\n");

            // Assert
            await this.AssertPlainTextRedactionOccurredFor(result, "\n");
        }

        [TestMethod]
        public async Task Can_use_carriage_return_and_newline_for_line_endings()
        {
            // Arrange
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            // Act
            RemoteWorkFile result = await prizmDocServer.RedactToPlainTextAsync("documents/confidential-contacts.pdf", "documents/confidential-contacts.pdf.markup.json", "\r\n");

            // Assert
            await this.AssertPlainTextRedactionOccurredFor(result, "\r\n");
        }

        [TestMethod]
        public async Task Can_use_local_file_path_for_document_and_RemoteWorkFile_for_markup_JSON()
        {
            // Arrange
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();
            RemoteWorkFile markupJson = await affinitySession.UploadAsync("documents/confidential-contacts.pdf.markup.json");

            // Act
            RemoteWorkFile result = await prizmDocServer.RedactToPlainTextAsync("documents/confidential-contacts.pdf", markupJson, "\n");

            // Assert
            await this.AssertPlainTextRedactionOccurredFor(result, "\n");
        }

        [TestMethod]
        public async Task Can_use_RemoteWorkFile_for_document_and_local_file_path_for_markup_JSON()
        {
            // Arrange
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();
            RemoteWorkFile document = await affinitySession.UploadAsync("documents/confidential-contacts.pdf");

            // Act
            RemoteWorkFile result = await prizmDocServer.RedactToPlainTextAsync(document, "documents/confidential-contacts.pdf.markup.json", "\n");

            // Assert
            await this.AssertPlainTextRedactionOccurredFor(result, "\n");
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
            RemoteWorkFile result = await prizmDocServer.RedactToPlainTextAsync(document, markupJson, "\n");

            // Assert
            await this.AssertPlainTextRedactionOccurredFor(result, "\n");
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
            RemoteWorkFile result = await prizmDocServer.RedactToPlainTextAsync(document, markupJson, "\n");

            // Assert
            await this.AssertPlainTextRedactionOccurredFor(result, "\n");
        }

        private async Task AssertPlainTextRedactionOccurredFor(RemoteWorkFile result, string expectedLineEndings)
        {
            using (var memoryStream = new MemoryStream())
            {
                await result.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                using (var reader = new StreamReader(memoryStream))
                {
                    string text = reader.ReadToEnd();

                    // Quick sanity check to verify redaction actually occurred
                    Assert.IsTrue(text.Contains("Peter Parker"), "Hmm, text content we expected to be in the output document was not present. Did something go wrong?");
                    Assert.IsFalse(text.Contains("hotshotpete@dailybugle.com"), "Content that was expected to be redacted was not actually redacted!");
                    Assert.IsTrue(text.Contains("<Text Redacted>"), "Expected to find an occurrence of the string \"<Text Redacted>\", but didn't!");

                    if (expectedLineEndings == "\r\n")
                    {
                        Assert.IsTrue(text.Contains("\r\n"));
                    }
                    else if (expectedLineEndings == "\n")
                    {
                        Assert.IsFalse(text.Contains("\r\n"));
                    }
                    else
                    {
                        throw new ArgumentException("expectedLineEndings must be either \"\\r\\n\" or \"\\n\".", "expectedLineEndings");
                    }
                }
            }
        }
    }
}
