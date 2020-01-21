using System;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class ConversionResult_Tests
    {
        [TestMethod]
        public void Constructor_rejects_null_remoteWorkFile()
        {
            UtilAssert.ThrowsExceptionWithMessageContaining<ArgumentNullException>(
                () => new ConversionResult(null, 1, new[] { new ConversionSourceDocument("example.pdf") }),
                "remoteWorkFile");
        }

        [TestMethod]
        public void Constructor_rejects_null_sources()
        {
            UtilAssert.ThrowsExceptionWithMessageContaining<ArgumentNullException>(
                () => new ConversionResult(new RemoteWorkFile(null, "fileId", "affinityToken", "pdf"), 1, null),
                "sources");
        }
    }
}
