using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Json.Serialization.Conversion.Tests
{
    [TestClass]
    public class HeaderFooterLineConverter_Tests
    {
        [TestMethod]
        public void ReadJson_is_not_implemented()
        {
            var converter = new HeaderFooterLineConverter();

            Assert.ThrowsException<NotImplementedException>(() => converter.ReadJson(null, null, null, null));
        }
    }
}
