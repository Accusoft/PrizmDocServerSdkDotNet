using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Conversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      var wf1 = await UploadPlainTextAsync(Util.RestClient.CreateAffinitySession(), "File 1");
      var wf2 = await UploadPlainTextAsync(Util.RestClient.CreateAffinitySession(), "File 2");

      // Make sure we get at least one distinct affinity token
      var i = 2;
      while (wf1.AffinityToken == wf2.AffinityToken && i < 100)
      {
        wf2 = await UploadPlainTextAsync(Util.RestClient.CreateAffinitySession(), $"File 2");
        i++;
      }

      // Try to create some files with the same affinity
      var affinitySession = Util.RestClient.CreateAffinitySession();
      var wf3 = await UploadPlainTextAsync(affinitySession, "File 3");
      var wf4 = await UploadPlainTextAsync(affinitySession, "File 4");
      var wf5 = await UploadPlainTextAsync(affinitySession, "File 5");
      var wf6 = await UploadPlainTextAsync(affinitySession, "File 6");
      var wf7 = await UploadPlainTextAsync(affinitySession, "File 7");
      var wf8 = await UploadPlainTextAsync(affinitySession, "File 8");
      var wf9 = await UploadPlainTextAsync(affinitySession, "File 9");
      var wf10 = await UploadPlainTextAsync(affinitySession, "File 10");

      var doc1 = new SourceDocument(wf1);
      var doc2 = new SourceDocument(wf2);
      var doc3 = new SourceDocument(wf3);
      var doc4 = new SourceDocument(wf4);
      var doc5 = new SourceDocument(wf5);
      var doc6 = new SourceDocument(wf6);
      var doc7 = new SourceDocument(wf7);
      var doc8 = new SourceDocument(wf8);
      var doc9 = new SourceDocument(wf9);
      var doc10 = new SourceDocument(wf10);

      var sourceDocuments = new [] { doc1, doc2, doc3, doc4, doc5, doc6, doc7, doc8, doc9, doc10 };

      // Validate that we actually have distinct affinity
      var distinctAffinityTokensBefore = sourceDocuments.Select(x => x.RemoteWorkFile.AffinityToken).Distinct();
      Assert.IsTrue(distinctAffinityTokensBefore.Count() > 1);

      var mostFrequentAffinityToken = sourceDocuments.GroupBy(x => x.RemoteWorkFile.AffinityToken).OrderByDescending(x => x.Count()).Select(x => x.Key).First();

      // Act
      var output = await Util.CreatePrizmDocServerClient().CombineToPdfAsync(sourceDocuments);

      // Assert that the SourceDocument instances all now have RemoteWorkFile instances with the same affinity token.
      var distinctAffinityTokensAfter = sourceDocuments.Select(x => x.RemoteWorkFile.AffinityToken).Distinct();
      Assert.AreEqual(1, distinctAffinityTokensAfter.Count());
      Assert.AreEqual(mostFrequentAffinityToken, distinctAffinityTokensAfter.Single());

      var outputFileText = String.Join("\n", await TextUtil.ExtractPagesText(output.RemoteWorkFile));
      Assert.AreEqual(@"File 1File 2File 3File 4File 5File 6File 7File 8File 9File 10", outputFileText.Replace("\r", "").Replace("\n", ""));
    }
  }
}
