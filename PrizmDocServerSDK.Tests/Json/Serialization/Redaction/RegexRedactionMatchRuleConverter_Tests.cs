using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Json.Serialization.Redaction.Tests
{
    [TestClass]
    public class RegexRedactionMatchRuleConverter_Tests
    {
        [TestMethod]
        public void ReadJson_is_not_implemented()
        {
            var converter = new RegexRedactionMatchRuleConverter();

            Assert.ThrowsException<NotImplementedException>(() => converter.ReadJson(null, null, null, null));
        }
    }
}
