using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Burning.KnownServerErrors.Tests
{
    [TestClass]
    public class MarkupFileContentInvalidAccordingToSchema_Tests
    {
        [TestMethod]
        public async Task BurnMarkupAsync_fails_with_a_useful_error_message_when_the_markup_json_file_contains_content_which_does_not_pass_schema_validation()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.BurnMarkupAsync("documents/confidential-contacts.pdf", "documents/content-fails-schema-validation.markup.json");
                }, "The remote server rejected the given markup JSON because it contained content which did not conform to its allowed markup JSON schema. See the markup JSON schema documentation for your version of PrizmDoc Viewer (such as https://help.accusoft.com/PrizmDoc/latest/HTML/webframe.html#markup-json-specification.html).");
        }
    }
}
