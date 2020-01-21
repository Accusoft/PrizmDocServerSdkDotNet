using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Tests
{
    [TestClass]
    public class RemoteWorkFile_ToString_Tests
    {
        [TestMethod]
        public void ToString_includes_FileId_AffinityToken_and_FileExtension()
        {
          string toStringValue = new RemoteWorkFile(null, "fileId", "affinityToken", "pdf").ToString();
          StringAssert.Contains(toStringValue, "FileId: fileId");
          StringAssert.Contains(toStringValue, "AffinityToken: affinityToken");
          StringAssert.Contains(toStringValue, "FileExtension: pdf");
        }

        [TestMethod]
        public void ToString_omits_AffinityToken_when_null()
        {
          string toStringValue = new RemoteWorkFile(null, "fileId", null, "pdf").ToString();
          StringAssert.Contains(toStringValue, "FileId: fileId");
          Assert.IsFalse(toStringValue.Contains("AffinityToken:"));
          StringAssert.Contains(toStringValue, "FileExtension: pdf");
        }

        [TestMethod]
        public void ToString_omits_FileExtension_when_null()
        {
          string toStringValue = new RemoteWorkFile(null, "fileId", "affinityToken", null).ToString();
          StringAssert.Contains(toStringValue, "FileId: fileId");
          StringAssert.Contains(toStringValue, "AffinityToken: affinityToken");
          Assert.IsFalse(toStringValue.Contains("FileExtension:"));
        }
    }
}
