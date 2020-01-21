using System;
using System.Collections.Generic;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class ConversionSourceDocument_Tests
    {
        [TestMethod]
        public void Constructor_rejects_null_remoteWorkFile()
        {
            UtilAssert.ThrowsExceptionWithMessageContaining<ArgumentNullException>(
                () => new ConversionSourceDocument((RemoteWorkFile)null),
                "remoteWorkFile");
        }

        [TestMethod]
        public void Can_easily_construct_a_list_of_conversion_inputs()
        {
            var inputs = new List<ConversionSourceDocument>
            {
                new ConversionSourceDocument("other.docx"),
                new ConversionSourceDocument("wat.pdf"),
                new ConversionSourceDocument("somefile.txt", pages: "1-2"),
                new ConversionSourceDocument("protected.pdf", password: "opensesame"),
                new ConversionSourceDocument("protected.pdf", pages: "1", password: "opensesame"),
                new ConversionSourceDocument(new RemoteWorkFile(null, fileId: "abc123", affinityToken: "1234", fileExtension: "docx")),
                new ConversionSourceDocument(new RemoteWorkFile(null, fileId: "abc123", affinityToken: "1234", fileExtension: "docx"), pages: "2-3"),
                new ConversionSourceDocument(new RemoteWorkFile(null, fileId: "abc123", affinityToken: "1234", fileExtension: "docx"), password: "letmein"),
                new ConversionSourceDocument(new RemoteWorkFile(null, fileId: "abc123", affinityToken: "1234", fileExtension: "docx"), pages: "1", password: "letmein"),
            };
        }
    }
}
