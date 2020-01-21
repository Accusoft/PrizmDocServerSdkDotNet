using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Json.Serialization.Conversion.Tests
{
    [TestClass]
    public class JpegDestinationOptionsConverter_Tests
    {
        [TestMethod]
        public void ReadJson_is_not_implemented()
        {
            var converter = new JpegDestinationOptionsConverter();

            Assert.ThrowsException<NotImplementedException>(() => converter.ReadJson(null, null, null, null));
        }
    }
}
