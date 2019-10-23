using System;
using Accusoft.PrizmDocServer.Conversion;
using Newtonsoft.Json;

namespace Accusoft.PrizmDocServer.Json.Serialization.Conversion
{
  public class TiffDestinationOptionsConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(TiffDestinationOptions);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var options = value as TiffDestinationOptions;

      writer.WriteStartObject();

      if (options.ForceOneFilePerPage)
      {
        writer.WritePropertyName("forceOneFilePerPage");
        writer.WriteValue(true);
      }

      if (options.MaxWidth != null)
      {
        writer.WritePropertyName("maxWidth");
        writer.WriteValue(options.MaxWidth);
      }

      if (options.MaxHeight != null)
      {
        writer.WritePropertyName("maxHeight");
        writer.WriteValue(options.MaxHeight);
      }

      writer.WriteEndObject();
    }
  }
}
