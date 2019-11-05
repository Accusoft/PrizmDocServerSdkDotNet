using System.Collections.Generic;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class Source_Pages_Invalid_Tests
    {
        [TestMethod]
        public async Task When_using_a_single_source_input()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(new SourceDocument("documents/example.pdf", pages: "wat"), new DestinationOptions(DestinationFileFormat.Pdf));
                }, "SourceDocument (\"documents/example.pdf\") has an invalid value for \"pages\". A valid pages value is a string like \"1\", \"1,3,5-10\", or \"2-\" (just like in a print dialog).");
        }

        [TestMethod]
        public async Task When_there_are_multiple_source_inputs()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(
                        new List<SourceDocument>
                        {
                            new SourceDocument("documents/example.pdf"),
                            new SourceDocument("documents/example.pdf", pages: "wat"),
                            new SourceDocument("documents/example.pdf"),
                        },
                        new DestinationOptions(DestinationFileFormat.Pdf));
                }, "SourceDocument at index 1 (\"documents/example.pdf\") has an invalid value for \"pages\". A valid pages value is a string like \"1\", \"1,3,5-10\", or \"2-\" (just like in a print dialog).");
        }
    }
}
