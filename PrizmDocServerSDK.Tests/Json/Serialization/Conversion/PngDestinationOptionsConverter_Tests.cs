using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Json.Serialization.Conversion.Tests
{
    [TestClass]
    public class PngDestinationOptionsConverter_Tests
    {
        [TestMethod]
        public void ReadJson_is_not_implemented()
        {
            var converter = new PngDestinationOptionsConverter();

            Assert.ThrowsException<NotImplementedException>(() => converter.ReadJson(null, null, null, null));
        }
    }
}
