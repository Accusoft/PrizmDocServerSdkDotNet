using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class ConvertAsync_Tiff_Tests
    {
        [TestMethod]
        public async Task Multiple_inputs_one_with_password()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();
            ConversionSourceDocument sourceDocument1 = new ConversionSourceDocument("documents/example.docx");
            ConversionSourceDocument sourceDocument2 = new ConversionSourceDocument("documents/password.docx", password: "open");
            ConversionResult result = (await prizmDocServer.ConvertAsync(new[] { sourceDocument1, sourceDocument2 }, DestinationFileFormat.Tiff)).Single();

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(3, result.PageCount);

            List<ConversionSourceDocument> resultSourceDocuments = result.Sources.ToList();

            Assert.AreEqual(sourceDocument1.RemoteWorkFile, resultSourceDocuments[0].RemoteWorkFile);
            Assert.IsNull(resultSourceDocuments[0].Password);
            Assert.AreEqual("1-2", resultSourceDocuments[0].Pages);

            Assert.AreEqual(sourceDocument2.RemoteWorkFile, resultSourceDocuments[1].RemoteWorkFile);
            Assert.IsNull(resultSourceDocuments[1].Password);
            Assert.AreEqual("1", resultSourceDocuments[1].Pages);

            await result.RemoteWorkFile.SaveAsync("output.tiff");
            FileAssert.IsTiff("output.tiff");
        }
    }
}
