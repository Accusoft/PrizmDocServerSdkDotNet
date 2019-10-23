using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Tests
{
  [TestClass]
  public class RemoteWorkFile_Equality_Tests
  {
    [TestMethod]
    public void Two_RemoteWorkFile_instances_are_equal_if_their_FileId_AffinityToken_and_FileExtension_are_equal()
    {
      Assert.AreEqual(
        new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf"),
        new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf")
      );
    }

    [TestMethod]
    public void Two_RemoteWorkFile_instances_are_NOT_equal_if_only_their_FileId_differs()
    {
      Assert.AreNotEqual(
        new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf"),
        new RemoteWorkFile(null, "fileId2", "affinityToken1", "pdf")
      );
    }

    [TestMethod]
    public void Two_RemoteWorkFile_instances_are_NOT_equal_if_only_their_AffinityToken_differs()
    {
      Assert.AreNotEqual(
        new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf"),
        new RemoteWorkFile(null, "fileId1", "affinityToken2", "pdf")
      );
    }

    [TestMethod]
    public void Two_RemoteWorkFile_instances_are_NOT_equal_if_only_their_FileExtension_differs()
    {
      Assert.AreNotEqual(
        new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf"),
        new RemoteWorkFile(null, "fileId1", "affinityToken1", null)
      );
    }
  }
}
