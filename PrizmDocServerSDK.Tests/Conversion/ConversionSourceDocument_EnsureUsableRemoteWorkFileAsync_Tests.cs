using System.IO;
using System.Text;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class ConversionSourceDocument_EnsureUsableRemoteWorkFileAsync_Tests
    {
        [TestMethod]
        public async Task Will_throw_an_exception_if_given_a_path_to_a_local_file_which_cannot_be_found()
        {
            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();
            var input = new ConversionSourceDocument("documents/does not exist.pdf");
            Assert.IsNull(input.RemoteWorkFile);

            await UtilAssert.ThrowsExceptionWithMessageAsync<FileNotFoundException>(
                async () => { await input.EnsureUsableRemoteWorkFileAsync(affinitySession); },
                "File not found: \"documents/does not exist.pdf\"");
        }

        [TestMethod]
        public async Task Will_POST_work_file_when_given_a_local_file_path()
        {
            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();
            var input = new ConversionSourceDocument("documents/example.docx");
            Assert.IsNull(input.RemoteWorkFile);
            await input.EnsureUsableRemoteWorkFileAsync(affinitySession);
            Assert.IsNotNull(input.RemoteWorkFile);
        }

        [TestMethod]
        public async Task Will_use_existing_RemoteWorkFile()
        {
            AffinitySession affinitySession = Util.RestClient.CreateAffinitySession();

            RemoteWorkFile remoteWorkFile;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello world!")))
            {
                remoteWorkFile = await affinitySession.UploadAsync(stream);
            }

            var input = new ConversionSourceDocument(remoteWorkFile);
            Assert.AreEqual(remoteWorkFile, input.RemoteWorkFile);
            await input.EnsureUsableRemoteWorkFileAsync(affinitySession);
            Assert.AreEqual(remoteWorkFile, input.RemoteWorkFile);
        }

        [MultiServerTestMethod]
        public async Task Will_reupload_an_existing_RemoteWorkFile_when_the_affinity_is_wrong()
        {
            AffinitySession session1 = Util.RestClient.CreateAffinitySession();
            AffinitySession session2 = Util.RestClient.CreateAffinitySession();

            RemoteWorkFile file1;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("File 1")))
            {
                file1 = await session1.UploadAsync(stream);
            }

            RemoteWorkFile file2;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("File 2")))
            {
                file2 = await session2.UploadAsync(stream);
            }

            Assert.AreNotEqual(file1.AffinityToken, file2.AffinityToken);

            var source2 = new ConversionSourceDocument(file2);
            RemoteWorkFile originalRemoteWorkFile = source2.RemoteWorkFile;
            Assert.AreEqual(file2, originalRemoteWorkFile);

            // Ensure file2 is re-uploaded to the same machine as file1...
            await source2.EnsureUsableRemoteWorkFileAsync(Util.RestClient.CreateAffinitySession(), affinityToken: file1.AffinityToken);

            // Verify source RemoteWorkFile assignment was changed to something new
            Assert.AreNotEqual(source2.RemoteWorkFile, originalRemoteWorkFile);

            // Verify the affinity token of file1 and source2.RemoteWorkFile now match
            Assert.AreEqual(file1.AffinityToken, source2.RemoteWorkFile.AffinityToken);

            // Verify the contents of the file are still correct
            using (var stream = new MemoryStream())
            {
                await source2.RemoteWorkFile.CopyToAsync(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string text = reader.ReadToEnd();
                    Assert.AreEqual("File 2", text);
                }
            }
        }
    }
}
