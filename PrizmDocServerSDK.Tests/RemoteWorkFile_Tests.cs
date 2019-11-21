using System.IO;
using System.Text;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Tests
{
    [TestClass]
    public class RemoteWorkFile_Tests
    {
        [TestMethod]
        public async Task UploadAsync_with_local_file_path_followed_by_SaveAsync_roundtrip_works()
        {
            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();

            const string INPUT_FILENAME = "documents/example.docx";
            const string OUTPUT_FILENAME = "downloaded.docx";

            RemoteWorkFile remoteWorkFile = await affinitySession.UploadAsync(INPUT_FILENAME);
            await remoteWorkFile.SaveAsync(OUTPUT_FILENAME);

            CollectionAssert.AreEqual(File.ReadAllBytes(INPUT_FILENAME), File.ReadAllBytes(OUTPUT_FILENAME));
        }

        [TestMethod]
        public async Task UploadAsync_with_memory_stream_followed_by_SaveAsync_roundtrip_works()
        {
            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();

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
            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();

            const string INPUT_FILENAME = "documents/example.docx";

            RemoteWorkFile remoteWorkFile = await affinitySession.UploadAsync(INPUT_FILENAME);

            using (var memoryStream = new MemoryStream())
            {
                await remoteWorkFile.CopyToAsync(memoryStream);
                CollectionAssert.AreEqual(File.ReadAllBytes(INPUT_FILENAME), memoryStream.ToArray());
            }
        }

        [MultiServerTestMethod]
        public async Task GetInstanceWithAffinity_works()
        {
            // Arrange
            AffinitySession session1 = Util.RestClient.CreateAffinitySession();
            AffinitySession session2 = Util.RestClient.CreateAffinitySession();

            RemoteWorkFile file1 = await session1.UploadAsync("documents/confidential-contacts.pdf");
            RemoteWorkFile file2 = await session2.UploadAsync("documents/confidential-contacts.pdf.markup.json");

            Assert.AreNotEqual(file1.AffinityToken, file2.AffinityToken);

            // Act
            RemoteWorkFile file2Reuploaded = await file2.GetInstanceWithAffinity(session2, file1.AffinityToken);

            // Assert
            Assert.AreEqual(file2.FileExtension, file2Reuploaded.FileExtension, "The FileExtension was not set correctly after reupload!");
            Assert.AreEqual(file1.AffinityToken, file2Reuploaded.AffinityToken, "The AffinityToken was not correct after reupload!");

            using (var originalContent = new MemoryStream())
            using (var reuploadedContent = new MemoryStream())
            {
                await file2.CopyToAsync(originalContent);
                await file2Reuploaded.CopyToAsync(reuploadedContent);

                CollectionAssert.AreEqual(originalContent.ToArray(), reuploadedContent.ToArray());
            }
        }
    }
}
