using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer.Redaction.Tests
{
    [TestClass]
    public class CreateRedactionsAsync_Tests
    {
        [TestMethod]
        public async Task Can_create_redactions()
        {
            // Arrange
            PrizmDocServerClient prizmDocServer = Util.CreatePrizmDocServerClient();

            var ssn = new RegexRedactionMatchRule(@"\d\d\d-\d\d-\d\d\d\d")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "(b)(6)",
                    Data = new Dictionary<string, string>
                    {
                        { "rule", "SSN" },
                    },
                },
            };

            var email = new RegexRedactionMatchRule(@"\S+@\S+\.\S+")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "(b)(6)",
                    Data = new Dictionary<string, string>
                    {
                        { "rule", "email" },
                    },
                },
            };

            var bruceWayne = new RegexRedactionMatchRule(@"Bruce Wayne")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "Not Batman",
                },
            };

            var rules = new[]
            {
                ssn,
                email,
                bruceWayne,
            };

            // Act
            RemoteWorkFile result = await prizmDocServer.CreateRedactionsAsync("documents/confidential-contacts.pdf", rules);

            // Assert: Verify the expected redactions were created for the test document
            JObject markup;
            using (var memoryStream = new MemoryStream())
            {
                await result.CopyToAsync(memoryStream);
                string markupJson = Encoding.ASCII.GetString(memoryStream.ToArray());
                markup = JObject.Parse(markupJson);
            }

            List<JToken> marks = markup["marks"].Children().ToList();
            List<JToken> redactions = marks.Where(x => (string)x["type"] == "RectangleRedaction").ToList();
            List<JToken> firstPageRedactions = redactions.Where(x => (int)x["pageNumber"] == 1).ToList();
            List<JToken> secondPageRedactions = redactions.Where(x => (int)x["pageNumber"] == 2).ToList();
            List<JToken> firstPageSsnRedactions = firstPageRedactions.Where(x => x["data"] != null && (string)x["data"]["rule"] == "SSN").ToList();
            List<JToken> secondPageSsnRedactions = secondPageRedactions.Where(x => x["data"] != null && (string)x["data"]["rule"] == "SSN").ToList();
            List<JToken> firstPageEmailRedactions = firstPageRedactions.Where(x => x["data"] != null && (string)x["data"]["rule"] == "email").ToList();
            List<JToken> secondPageEmailRedactions = secondPageRedactions.Where(x => x["data"] != null && (string)x["data"]["rule"] == "email").ToList();
            List<JToken> bruceWayneRedactions = redactions.Where(x => (string)x["reason"] == "Not Batman").ToList();

            Assert.AreEqual(18, marks.Count);
            Assert.AreEqual(18, redactions.Count);
            Assert.AreEqual(13, firstPageRedactions.Count);
            Assert.AreEqual(5, secondPageRedactions.Count);
            Assert.AreEqual(6, firstPageSsnRedactions.Count);
            Assert.AreEqual(3, secondPageSsnRedactions.Count);
            Assert.AreEqual(6, firstPageEmailRedactions.Count);
            Assert.AreEqual(2, secondPageEmailRedactions.Count);
            Assert.AreEqual(1, bruceWayneRedactions.Count);
        }
    }
}
