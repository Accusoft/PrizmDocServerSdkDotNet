using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Redaction.KnownServerErrors.Tests
{
    [TestClass]
    public class BorderColor_Invalid_Tests
    {
        [TestMethod]
        public async Task Single_rule_with_invalid_BorderColor()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            var rules = new[]
                {
                    new RegexRedactionMatchRule(@"\d\d\d-\d\d-\d\d\d\d")
                    {
                        RedactWith = new RedactionCreationOptions()
                        {
                            BorderColor = "Bluuuuue",
                        },
                    },
                };

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.CreateRedactionsAsync("documents/confidential-contacts.pdf", rules);
                }, "RedactionMatchRule has invalid RedactWith.BorderColor for remote server: \"Bluuuuue\"");
        }

        [TestMethod]
        public async Task Multiple_rules_where_one_has_invalid_BorderColor()
        {
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            var rules = new[]
                {
                    new RegexRedactionMatchRule(@"\d\d\d-\d\d-\d\d\d\d"),
                    new RegexRedactionMatchRule(@"\S+@acme\.com")
                    {
                        RedactWith = new RedactionCreationOptions()
                        {
                            BorderColor = "REDDD",
                        },
                    },
                };

            await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(
                async () =>
                {
                    await prizmDocServer.CreateRedactionsAsync("documents/confidential-contacts.pdf", rules);
                }, "RedactionMatchRule at index 1 has invalid RedactWith.BorderColor for remote server: \"REDDD\"");
        }
    }
}
