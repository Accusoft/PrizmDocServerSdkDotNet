using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Conversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Tests
{
    [TestClass]
    public class AffinityTests
    {
        public async Task<RemoteWorkFile> UploadPlainTextAsync(AffinitySession affinitySession, string text)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return await affinitySession.UploadAsync(stream);
            }
        }

        [MultiServerTestMethod]
        public async Task Work_file_reuploading_works_correctly_and_number_of_reuploads_is_minimized()
        {
            // Arrange
            RemoteWorkFile wf1 = await this.UploadPlainTextAsync(Util.RestClient.CreateAffinitySession(), "File 1");
            RemoteWorkFile wf2 = await this.UploadPlainTextAsync(Util.RestClient.CreateAffinitySession(), "File 2");

            // Make sure we get at least one distinct affinity token
            int i = 2;
            while (wf1.AffinityToken == wf2.AffinityToken && i < 100)
            {
                wf2 = await this.UploadPlainTextAsync(Util.RestClient.CreateAffinitySession(), $"File 2");
                i++;
            }

            // Try to create some files with the same affinity
            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();
            RemoteWorkFile wf3 = await this.UploadPlainTextAsync(affinitySession, "File 3");
            RemoteWorkFile wf4 = await this.UploadPlainTextAsync(affinitySession, "File 4");
            RemoteWorkFile wf5 = await this.UploadPlainTextAsync(affinitySession, "File 5");
            RemoteWorkFile wf6 = await this.UploadPlainTextAsync(affinitySession, "File 6");
            RemoteWorkFile wf7 = await this.UploadPlainTextAsync(affinitySession, "File 7");
            RemoteWorkFile wf8 = await this.UploadPlainTextAsync(affinitySession, "File 8");
            RemoteWorkFile wf9 = await this.UploadPlainTextAsync(affinitySession, "File 9");
            RemoteWorkFile wf10 = await this.UploadPlainTextAsync(affinitySession, "File 10");

            var doc1 = new ConversionSourceDocument(wf1);
            var doc2 = new ConversionSourceDocument(wf2);
            var doc3 = new ConversionSourceDocument(wf3);
            var doc4 = new ConversionSourceDocument(wf4);
            var doc5 = new ConversionSourceDocument(wf5);
            var doc6 = new ConversionSourceDocument(wf6);
            var doc7 = new ConversionSourceDocument(wf7);
            var doc8 = new ConversionSourceDocument(wf8);
            var doc9 = new ConversionSourceDocument(wf9);
            var doc10 = new ConversionSourceDocument(wf10);

            var sourceDocuments = new[] { doc1, doc2, doc3, doc4, doc5, doc6, doc7, doc8, doc9, doc10 };

            // Validate that we actually have distinct affinity
            IEnumerable<string> distinctAffinityTokensBefore = sourceDocuments.Select(x => x.RemoteWorkFile.AffinityToken).Distinct();
            Assert.IsTrue(distinctAffinityTokensBefore.Count() > 1);

            string mostFrequentAffinityToken = sourceDocuments.GroupBy(x => x.RemoteWorkFile.AffinityToken).OrderByDescending(x => x.Count()).Select(x => x.Key).First();

            // Act
            ConversionResult output = await Util.CreatePrizmDocServerClient().CombineToPdfAsync(sourceDocuments);

            // Assert that the ConversionSourceDocument instances all now have RemoteWorkFile instances with the same affinity token.
            IEnumerable<string> distinctAffinityTokensAfter = sourceDocuments.Select(x => x.RemoteWorkFile.AffinityToken).Distinct();
            Assert.AreEqual(1, distinctAffinityTokensAfter.Count());
            Assert.AreEqual(mostFrequentAffinityToken, distinctAffinityTokensAfter.Single());

            string outputFileText = string.Join("\n", await TextUtil.ExtractPagesText(output.RemoteWorkFile));
            Assert.AreEqual(@"File 1File 2File 3File 4File 5File 6File 7File 8File 9File 10", outputFileText.Replace("\r", string.Empty).Replace("\n", string.Empty));
        }
    }
}
