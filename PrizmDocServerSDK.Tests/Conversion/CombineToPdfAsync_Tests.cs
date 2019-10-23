using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accusoft.PrizmDocServer.Tests;
using System.Linq;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
  [TestClass]
  public class CombineToPdfAsync_Tests
  {
    [TestMethod]
    public async Task Multiple_inputs_one_with_password()
    {
      var context = Util.CreateContext();
      var sourceDocument1 = new SourceDocument("documents/example.docx");
      var sourceDocument2 = new SourceDocument("documents/password.docx", password: "open");
      var result = await context.CombineToPdfAsync(new[] { sourceDocument1, sourceDocument2 });
      Assert.IsTrue(result.IsSuccess);
      Assert.AreEqual(3, result.PageCount);

      var resultSourceDocuments = result.Sources.ToList();

      Assert.AreEqual(sourceDocument1.RemoteWorkFile, resultSourceDocuments[0].RemoteWorkFile);
      Assert.IsNull(resultSourceDocuments[0].Password);
      Assert.AreEqual("1-2", resultSourceDocuments[0].Pages);

      Assert.AreEqual(sourceDocument2.RemoteWorkFile, resultSourceDocuments[1].RemoteWorkFile);
      Assert.IsNull(resultSourceDocuments[1].Password);
      Assert.AreEqual("1", resultSourceDocuments[1].Pages);

      await result.RemoteWorkFile.SaveAsync("output.pdf");
      FileAssert.IsPdf("output.pdf");
    }
  }
}
