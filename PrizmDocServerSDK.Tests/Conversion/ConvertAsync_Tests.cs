using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class ConvertAsync_Tests
    {
        [TestMethod]
        public async Task Can_perform_a_conversion()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            IEnumerable<Result> results = await prizmDocServer.ConvertAsync(
                new List<SourceDocument>
                {
                    new SourceDocument("documents/example.docx", pages: "1"),
                    new SourceDocument("documents/example.docx"),
                },
                new DestinationOptions(DestinationFileFormat.Pdf));

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(3, results.Single().PageCount);
            Assert.AreEqual("1", results.Single().Sources.ToList()[0].Pages);
            Assert.AreEqual("1-2", results.Single().Sources.ToList()[1].Pages);

            await results.Single().RemoteWorkFile.SaveAsync("output.pdf");
            FileAssert.IsPdf("output.pdf");
        }

        [TestMethod]
        public async Task Document_passwords_are_not_included_in_results()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            IEnumerable<Result> results = await prizmDocServer.ConvertAsync(new SourceDocument("documents/password.docx", password: "open"), new DestinationOptions(DestinationFileFormat.Pdf));

            Assert.IsNull(results.Single().Sources.Single().Password);
        }
    }
}
