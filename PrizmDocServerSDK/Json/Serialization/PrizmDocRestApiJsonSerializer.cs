using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Accusoft.PrizmDocServer.Json.Serialization
{
  public static class PrizmDocRestApiJsonSerializer
  {
    private static readonly JsonSerializer instance;

    static PrizmDocRestApiJsonSerializer()
    {
      instance = new JsonSerializer {
        ContractResolver = new DefaultContractResolver {
          NamingStrategy = new CamelCaseNamingStrategy()
        },
        NullValueHandling = NullValueHandling.Ignore,
      };
      instance.Converters.Add(new StringEnumConverter() {
        NamingStrategy = new CamelCaseNamingStrategy()
      });
      instance.Converters.Add(new Conversion.FormatConverter());
      instance.Converters.Add(new Conversion.JpegDestinationOptionsConverter());
      instance.Converters.Add(new Conversion.PngDestinationOptionsConverter());
      instance.Converters.Add(new Conversion.TiffDestinationOptionsConverter());
      instance.Converters.Add(new Conversion.HeaderFooterLineConverter());
    }

    public static JsonSerializer Instance => instance;
  }
}
