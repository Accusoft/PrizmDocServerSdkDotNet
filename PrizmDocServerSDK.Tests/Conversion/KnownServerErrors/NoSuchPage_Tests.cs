using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class NoSuchPage_Tests
    {
        [TestMethod]
        public async Task When_using_a_single_source_input()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            var sourceDocument = new SourceDocument("documents/example.pdf", pages: "97-99");

            IEnumerable<Result> results = await prizmDocServer.ConvertAsync(sourceDocument, new DestinationOptions(DestinationFileFormat.Pdf)
            {
                PdfOptions = new PdfDestinationOptions()
                {
                    ForceOneFilePerPage = true,
                },
            });

            Assert.AreEqual(3, results.Count());

            AssertErrorResult(results.ElementAt(0), "NoSuchPage", "97", sourceDocument);
            AssertErrorResult(results.ElementAt(1), "NoSuchPage", "98", sourceDocument);
            AssertErrorResult(results.ElementAt(2), "NoSuchPage", "99", sourceDocument);
        }

        [TestMethod]
        public async Task When_there_are_multiple_source_inputs()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            var source1 = new SourceDocument("documents/example.pdf");
            var source2 = new SourceDocument("documents/example.pdf", pages: "97-99");
            var source3 = new SourceDocument("documents/example.pdf");

            IEnumerable<Result> results = await prizmDocServer.ConvertAsync(
                new List<SourceDocument>
                {
                    source1,
                    source2,
                    source3,
                },
                new DestinationOptions(DestinationFileFormat.Pdf)
                {
                    PdfOptions = new PdfDestinationOptions()
                    {
                        ForceOneFilePerPage = true,
                    },
                });

            Assert.AreEqual(7, results.Count());

            AssertSuccessResult(results.ElementAt(0), "1", source1);
            AssertSuccessResult(results.ElementAt(1), "2", source1);
            AssertErrorResult(results.ElementAt(2), "NoSuchPage", "97", source2);
            AssertErrorResult(results.ElementAt(3), "NoSuchPage", "98", source2);
            AssertErrorResult(results.ElementAt(4), "NoSuchPage", "99", source2);
            AssertSuccessResult(results.ElementAt(5), "1", source3);
            AssertSuccessResult(results.ElementAt(6), "2", source3);
        }

        private static void AssertSuccessResult(Result result, string expectedPagesValue, SourceDocument associatedSourceDocument, int expectedPageCount = 1)
        {
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.RemoteWorkFile);
            Assert.AreEqual(expectedPageCount, result.PageCount);
            Assert.IsNull(result.ErrorCode);
            Assert.AreEqual(expectedPagesValue, result.Sources.ElementAt(0).Pages);
            Assert.AreEqual(associatedSourceDocument.RemoteWorkFile.FileId, result.Sources.ElementAt(0).RemoteWorkFile.FileId);
            Assert.AreEqual(associatedSourceDocument.RemoteWorkFile.FileExtension, result.Sources.ElementAt(0).RemoteWorkFile.FileExtension);
        }

        private static void AssertErrorResult(Result result, string expectedErrorCode, string expectedPagesValue, SourceDocument associatedSourceDocument)
        {
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsError);

            // We want to throw if someone tries to get the RemoteWorkFile from an error result,
            // because we don't want them storing null to some var and then use it much later
            // only to discover they can't. So we fail fast.
            Assert.ThrowsException<InvalidOperationException>(() => { var x = result.RemoteWorkFile; });

            Assert.AreEqual(null, result.PageCount);
            Assert.AreEqual(expectedErrorCode, result.ErrorCode);
            Assert.AreEqual(1, result.Sources.Count());
            Assert.AreEqual(expectedPagesValue, result.Sources.ElementAt(0).Pages);
            Assert.AreEqual(associatedSourceDocument.RemoteWorkFile.FileId, result.Sources.ElementAt(0).RemoteWorkFile.FileId);
            Assert.AreEqual(associatedSourceDocument.RemoteWorkFile.FileExtension, result.Sources.ElementAt(0).RemoteWorkFile.FileExtension);
        }
    }
}
