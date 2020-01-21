using System.Collections.Generic;
using System.IO;
using Accusoft.PrizmDocServer.Redaction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Json.Serialization.Redaction.Tests
{
    [TestClass]
    public class RegexRedactionMatchRuleSerialization_Tests
    {
        [TestMethod]
        public void Serializes_nothing_when_pattern_is_null()
        {
            var rule = new RegexRedactionMatchRule();

            using (var stringWriter = new StringWriter())
            {
                PrizmDocRestApiJsonSerializer.Instance.Serialize(stringWriter, rule);
                Assert.AreEqual(string.Empty, stringWriter.ToString());
            }
        }

        [TestMethod]
        public void Can_serialize_a_minimal_instance()
        {
            var rule = new RegexRedactionMatchRule("My Pattern");

            using (var stringWriter = new StringWriter())
            {
                PrizmDocRestApiJsonSerializer.Instance.Serialize(stringWriter, rule);
                Assert.AreEqual("{\"find\":{\"type\":\"regex\",\"pattern\":\"My Pattern\"},\"redactWith\":{\"type\":\"RectangleRedaction\"}}", stringWriter.ToString());
            }
        }

        [TestMethod]
        public void Can_serialize_a_fully_detailed_instance()
        {
            var rule = new RegexRedactionMatchRule("My Pattern")
            {
                RedactWith = new RedactionCreationOptions()
                {
                    Reason = "HIGHLY CONFIDENTIAL",
                    FillColor = "#FF0000",
                    BorderColor = "#00FF00",
                    FontColor = "#0000FF",
                    BorderThickness = 2,
                    Data = new Dictionary<string, string>
                    {
                        { "key1", "value1" },
                        { "key2", "value2" },
                    },
                },
            };

            using (var stringWriter = new StringWriter())
            {
                PrizmDocRestApiJsonSerializer.Instance.Serialize(stringWriter, rule);
                Assert.AreEqual("{\"find\":{\"type\":\"regex\",\"pattern\":\"My Pattern\"},\"redactWith\":{\"type\":\"RectangleRedaction\",\"reason\":\"HIGHLY CONFIDENTIAL\",\"fontColor\":\"#0000FF\",\"fillColor\":\"#FF0000\",\"borderColor\":\"#00FF00\",\"borderThickness\":2,\"data\":{\"key1\":\"value1\",\"key2\":\"value2\"}}}", stringWriter.ToString());
            }
        }
    }
}
