using System.Collections.Generic;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
    [TestClass]
    public class PasswordProtectedDocx_Tests
    {
        [TestMethod]
        public async Task When_using_a_single_password_protected_source_document_and_no_password_is_given()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(new SourceDocument("documents/password.docx"), new DestinationOptions(DestinationFileFormat.Pdf));
                }, "Password required for SourceDocument (\"documents/password.docx\").");
        }

        [TestMethod]
        public async Task When_using_a_single_password_protected_source_document_and_the_wrong_password_is_given()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(new SourceDocument("documents/password.docx", password: "wrong"), new DestinationOptions(DestinationFileFormat.Pdf));
                }, "Invalid password for SourceDocument (\"documents/password.docx\").");
        }

        [TestMethod]
        public async Task When_the_password_protected_source_document_is_one_of_several_source_documents_and_no_password_is_given()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(
                        new List<SourceDocument>
                        {
                            new SourceDocument("documents/example.pdf"),
                            new SourceDocument("documents/password.docx"),
                            new SourceDocument("documents/example.pdf"),
                        },
                        new DestinationOptions(DestinationFileFormat.Pdf));
                }, "Password required for SourceDocument at index 1 (\"documents/password.docx\").");
        }

        [TestMethod]
        public async Task When_the_password_protected_source_document_is_one_of_several_source_documents_and_the_wrong_password_is_given()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.ConvertAsync(
                        new List<SourceDocument>
                        {
                            new SourceDocument("documents/example.pdf"),
                            new SourceDocument("documents/password.docx", password: "wrong"),
                            new SourceDocument("documents/example.pdf"),
                        },
                        new DestinationOptions(DestinationFileFormat.Pdf));
                }, "Invalid password for SourceDocument at index 1 (\"documents/password.docx\").");
        }
    }
}
