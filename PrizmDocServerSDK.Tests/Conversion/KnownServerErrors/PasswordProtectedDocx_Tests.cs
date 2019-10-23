using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accusoft.PrizmDocServer.Tests;
using Accusoft.PrizmDocServer.Exceptions;
using System.Collections.Generic;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
  [TestClass]
  public class PasswordProtectedDocx_Tests
  {
    [TestMethod]
    public async Task When_using_a_single_password_protected_source_document_and_no_password_is_given()
    {
      var context = Util.CreateContext();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await context.ConvertAsync(new SourceDocument("documents/password.docx"), new DestinationOptions(DestinationFileFormat.Pdf));
      }, "Password required for SourceDocument (\"documents/password.docx\").");
    }

    [TestMethod]
    public async Task When_using_a_single_password_protected_source_document_and_the_wrong_password_is_given()
    {
      var context = Util.CreateContext();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await context.ConvertAsync(new SourceDocument("documents/password.docx", password: "wrong"), new DestinationOptions(DestinationFileFormat.Pdf));
      }, "Invalid password for SourceDocument (\"documents/password.docx\").");
    }

    [TestMethod]
    public async Task When_the_password_protected_source_document_is_one_of_several_source_documents_and_no_password_is_given()
    {
      var context = Util.CreateContext();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await context.ConvertAsync(new List<SourceDocument> {
          new SourceDocument("documents/example.pdf"),
          new SourceDocument("documents/password.docx"),
          new SourceDocument("documents/example.pdf"),
        }, new DestinationOptions(DestinationFileFormat.Pdf));
      }, "Password required for SourceDocument at index 1 (\"documents/password.docx\").");
    }

    [TestMethod]
    public async Task When_the_password_protected_source_document_is_one_of_several_source_documents_and_the_wrong_password_is_given()
    {
      var context = Util.CreateContext();

      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await context.ConvertAsync(new List<SourceDocument> {
          new SourceDocument("documents/example.pdf"),
          new SourceDocument("documents/password.docx", password: "wrong"),
          new SourceDocument("documents/example.pdf"),
        }, new DestinationOptions(DestinationFileFormat.Pdf));
      }, "Invalid password for SourceDocument at index 1 (\"documents/password.docx\").");
    }
  }
}
