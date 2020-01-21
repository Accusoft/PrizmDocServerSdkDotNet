using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Tests
{
    [TestClass]
    public class RemoteWorkFile_Equality_Tests
    {
        [TestMethod]
        public void An_instance_compared_to_null_is_considered_not_equal()
        {
          Assert.AreNotEqual(new RemoteWorkFile(null, "fileId", "affinityToken", "pdf"), null);
        }

        [TestMethod]
        public void An_instance_compared_to_a_different_type_is_considered_not_equal()
        {
          Assert.AreNotEqual(new RemoteWorkFile(null, "fileId", "affinityToken", "pdf"), DateTime.Now);
        }

        [TestMethod]
        public void Two_RemoteWorkFile_instances_are_considered_equal_if_their_FileId_AffinityToken_and_FileExtension_are_equal()
        {
            Assert.AreEqual(
              new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf"),
              new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf"));
        }

        [TestMethod]
        public void Two_RemoteWorkFile_instances_return_the_same_hash_code_if_their_FileId_AffinityToken_and_FileExtension_are_equal()
        {
            Assert.AreEqual(
              new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf").GetHashCode(),
              new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf").GetHashCode());
        }

        [TestMethod]
        public void Two_RemoteWorkFile_instances_are_NOT_considered_equal_if_only_their_FileId_differs()
        {
            Assert.AreNotEqual(
              new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf"),
              new RemoteWorkFile(null, "fileId2", "affinityToken1", "pdf"));
        }

        [TestMethod]
        public void Two_RemoteWorkFile_instances_do_NOT_return_the_same_hash_code_if_only_their_FileId_differs()
        {
            Assert.AreNotEqual(
              new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf").GetHashCode(),
              new RemoteWorkFile(null, "fileId2", "affinityToken1", "pdf").GetHashCode());
        }

        [TestMethod]
        public void Two_RemoteWorkFile_instances_are_NOT_considered_equal_if_only_their_AffinityToken_differs()
        {
            Assert.AreNotEqual(
              new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf"),
              new RemoteWorkFile(null, "fileId1", "affinityToken2", "pdf"));
        }

        [TestMethod]
        public void Two_RemoteWorkFile_instances_do_NOT_return_the_same_hash_code_if_only_their_AffinityToken_differs()
        {
            Assert.AreNotEqual(
              new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf").GetHashCode(),
              new RemoteWorkFile(null, "fileId1", "affinityToken2", "pdf").GetHashCode());
        }

        [TestMethod]
        public void Two_RemoteWorkFile_instances_are_NOT_considered_equal_if_only_their_FileExtension_differs()
        {
            Assert.AreNotEqual(
              new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf"),
              new RemoteWorkFile(null, "fileId1", "affinityToken1", null));
        }

        [TestMethod]
        public void Two_RemoteWorkFile_instances_do_NOT_return_the_same_hash_code_if_only_their_FileExtension_differs()
        {
            Assert.AreNotEqual(
              new RemoteWorkFile(null, "fileId1", "affinityToken1", "pdf").GetHashCode(),
              new RemoteWorkFile(null, "fileId1", "affinityToken1", null).GetHashCode());
        }
    }
}
