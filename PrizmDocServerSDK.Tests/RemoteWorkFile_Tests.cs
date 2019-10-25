using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Accusoft.PrizmDocServer.Tests
{
  [TestClass]
  public class RemoteWorkFile_Tests
  {
    [TestMethod]
    public async Task UploadAsync_with_local_file_path_followed_by_SaveAsync_roundtrip_works()
    {
      var affinitySession = Util.RestClient.CreateAffinitySession();

      const string INPUT_FILENAME = "documents/example.docx";
      const string OUTPUT_FILENAME = "downloaded.docx";

      var remoteWorkFile = await affinitySession.UploadAsync(INPUT_FILENAME);
      await remoteWorkFile.SaveAsync(OUTPUT_FILENAME);

      CollectionAssert.AreEqual(File.ReadAllBytes(INPUT_FILENAME), File.ReadAllBytes(OUTPUT_FILENAME));
    }

    [TestMethod]
    public async Task UploadAsync_with_memory_stream_followed_by_SaveAsync_roundtrip_works()
    {
      var affinitySession = Util.RestClient.CreateAffinitySession();

      const string ORIGINAL_DOCUMENT_CONTENTS = "Hello world";
      const string OUTPUT_FILENAME = "downloaded.txt";

      RemoteWorkFile remoteWorkFile;
      using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(ORIGINAL_DOCUMENT_CONTENTS)))
      {
        remoteWorkFile = await affinitySession.UploadAsync(stream);
      }

      await remoteWorkFile.SaveAsync(OUTPUT_FILENAME);

      Assert.AreEqual(ORIGINAL_DOCUMENT_CONTENTS, File.ReadAllText(OUTPUT_FILENAME));
    }

    [TestMethod]
    public async Task UploadAsync_with_local_file_path_followed_by_CopyToAsync_roundtrip_works()
    {
      var affinitySession = Util.RestClient.CreateAffinitySession();

      const string INPUT_FILENAME = "documents/example.docx";

      var remoteWorkFile = await affinitySession.UploadAsync(INPUT_FILENAME);

      using (var memoryStream = new MemoryStream())
      {
        await remoteWorkFile.CopyToAsync(memoryStream);
        CollectionAssert.AreEqual(File.ReadAllBytes(INPUT_FILENAME), memoryStream.ToArray());
      }
    }
  }
}
