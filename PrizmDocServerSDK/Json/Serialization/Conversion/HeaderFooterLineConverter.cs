using System;
using Accusoft.PrizmDocServer.Conversion;
using Newtonsoft.Json;

namespace Accusoft.PrizmDocServer.Json.Serialization.Conversion
{
  public class HeaderFooterLineConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(HeaderFooterLine);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var line = value as HeaderFooterLine;

      writer.WriteStartArray();

      writer.WriteValue(line.Left ?? "");
      writer.WriteValue(line.Center ?? "");
      writer.WriteValue(line.Right ?? "");

      writer.WriteEndArray();
    }
  }
}
