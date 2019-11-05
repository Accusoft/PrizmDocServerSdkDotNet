using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class SourceDocument_Tests
    {
        [TestMethod]
        public void Can_easily_construct_a_list_of_conversion_inputs()
        {
            var inputs = new List<SourceDocument>
            {
                new SourceDocument("other.docx"),
                new SourceDocument("wat.pdf"),
                new SourceDocument("somefile.txt", pages: "1-2"),
                new SourceDocument("protected.pdf", password: "opensesame"),
                new SourceDocument("protected.pdf", pages: "1", password: "opensesame"),
                new SourceDocument(new RemoteWorkFile(null, fileId: "abc123", affinityToken: "1234", fileExtension: "docx")),
                new SourceDocument(new RemoteWorkFile(null, fileId: "abc123", affinityToken: "1234", fileExtension: "docx"), pages: "2-3"),
                new SourceDocument(new RemoteWorkFile(null, fileId: "abc123", affinityToken: "1234", fileExtension: "docx"), password: "letmein"),
                new SourceDocument(new RemoteWorkFile(null, fileId: "abc123", affinityToken: "1234", fileExtension: "docx"), pages: "1", password: "letmein"),
            };
        }
    }
}
