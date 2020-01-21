#pragma warning disable SA1600 // Elements should be documented

using System;
using Accusoft.PrizmDocServer.Conversion;
using Newtonsoft.Json;

namespace Accusoft.PrizmDocServer.Json.Serialization.Conversion
{
    public class DestinationFileFormatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DestinationFileFormat);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
}
