using System;
using System.IO;
using Accusoft.PrizmDocServer.Conversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Accusoft.PrizmDocServer.Json.Serialization.Conversion.Tests
{
    [TestClass]
    public class DestinationFileFormatConverter_Tests
    {
        [TestMethod]
        public void ReadJson_is_not_implemented()
        {
            var converter = new DestinationFileFormatConverter();

            Assert.ThrowsException<NotImplementedException>(() => converter.ReadJson(null, null, null, null));
        }

        [TestMethod]
        public void WriteJson_simply_writes_the_enum_value_as_an_all_lowercase_double_quoted_JSON_string_value()
        {
            this.AssertSerialization(DestinationFileFormat.Docx, "\"docx\"");
            this.AssertSerialization(DestinationFileFormat.Jpeg, "\"jpeg\"");
            this.AssertSerialization(DestinationFileFormat.Pdf, "\"pdf\"");
            this.AssertSerialization(DestinationFileFormat.Png, "\"png\"");
            this.AssertSerialization(DestinationFileFormat.Svg, "\"svg\"");
            this.AssertSerialization(DestinationFileFormat.Tiff, "\"tiff\"");
        }

        private void AssertSerialization(DestinationFileFormat input, string expectedOutput)
        {
            var converter = new DestinationFileFormatConverter();
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                converter.WriteJson(jsonWriter, input, null);
                Assert.AreEqual(expectedOutput, stringWriter.ToString());
            }
        }
    }
}
