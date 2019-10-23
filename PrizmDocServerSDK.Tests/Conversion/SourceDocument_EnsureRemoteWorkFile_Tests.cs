using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
  [TestClass]
  public class SourceDocument_EnsureRemoteWorkFile_Tests
  {
    [TestMethod]
    public async Task Will_POST_work_file_when_given_a_local_file_path()
    {
      var context = Util.CreateContext();

      var input = new SourceDocument("documents/example.docx");
      Assert.IsNull(input.RemoteWorkFile);
      await input.EnsureRemoteWorkFileAsync(context);
      Assert.IsNotNull(input.RemoteWorkFile);
    }

    [TestMethod]
    public async Task Will_use_existing_RemoteWorkFile()
    {
      var context = Util.CreateContext();

      RemoteWorkFile remoteWorkFile;
      using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello world!")))
      {
        remoteWorkFile = await context.UploadAsync(stream);
      }

      var input = new SourceDocument(remoteWorkFile);
      Assert.AreEqual(remoteWorkFile, input.RemoteWorkFile);
      await input.EnsureRemoteWorkFileAsync(context);
      Assert.AreEqual(remoteWorkFile, input.RemoteWorkFile);
    }
  }
}
