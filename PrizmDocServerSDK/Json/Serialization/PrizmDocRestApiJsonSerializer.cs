#pragma warning disable SA1600 // Elements should be documented

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Accusoft.PrizmDocServer.Json.Serialization
{
    public static class PrizmDocRestApiJsonSerializer
    {
        static PrizmDocRestApiJsonSerializer()
        {
            Instance = new JsonSerializer
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
                NullValueHandling = NullValueHandling.Ignore,
            };
            Instance.Converters.Add(new StringEnumConverter()
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            });
            Instance.Converters.Add(new Conversion.FormatConverter());
            Instance.Converters.Add(new Conversion.JpegDestinationOptionsConverter());
            Instance.Converters.Add(new Conversion.PngDestinationOptionsConverter());
            Instance.Converters.Add(new Conversion.TiffDestinationOptionsConverter());
            Instance.Converters.Add(new Conversion.HeaderFooterLineConverter());
        }

        public static JsonSerializer Instance { get; private set; }
    }
}
