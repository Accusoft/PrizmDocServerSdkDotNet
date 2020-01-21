#pragma warning disable SA1600 // Elements should be documented

using System;
using System.Collections.Generic;
using Accusoft.PrizmDocServer.Redaction;
using Newtonsoft.Json;

namespace Accusoft.PrizmDocServer.Json.Serialization.Redaction
{
    public class RegexRedactionMatchRuleConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RegexRedactionMatchRule);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rule = value as RegexRedactionMatchRule;

            // Don't serialize if there is no regex pattern defined.
            if (rule.Pattern == null)
            {
                return;
            }

            writer.WriteStartObject();

            // find
            writer.WritePropertyName("find");
            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue("regex");
            writer.WritePropertyName("pattern");
            writer.WriteValue(rule.Pattern);

            writer.WriteEndObject(); // end find object

            // redactWith
            writer.WritePropertyName("redactWith");
            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue("RectangleRedaction");

            if (rule.RedactWith.Reason != null)
            {
                writer.WritePropertyName("reason");
                writer.WriteValue(rule.RedactWith.Reason);
            }

            if (rule.RedactWith.FontColor != null)
            {
                writer.WritePropertyName("fontColor");
                writer.WriteValue(rule.RedactWith.FontColor);
            }

            if (rule.RedactWith.FillColor != null)
            {
                writer.WritePropertyName("fillColor");
                writer.WriteValue(rule.RedactWith.FillColor);
            }

            if (rule.RedactWith.BorderColor != null)
            {
                writer.WritePropertyName("borderColor");
                writer.WriteValue(rule.RedactWith.BorderColor);
            }

            if (rule.RedactWith.BorderThickness.HasValue)
            {
                writer.WritePropertyName("borderThickness");
                writer.WriteValue(rule.RedactWith.BorderThickness.Value);
            }

            // data
            var keysWithNonNullValues = new List<string>();

            foreach (string key in rule.RedactWith.Data.Keys)
            {
                if (rule.RedactWith.Data[key] != null)
                {
                    keysWithNonNullValues.Add(key);
                }
            }

            if (keysWithNonNullValues.Count > 0)
            {
                writer.WritePropertyName("data");
                writer.WriteStartObject();

                foreach (string key in keysWithNonNullValues)
                {
                    writer.WritePropertyName(key);
                    writer.WriteValue(rule.RedactWith.Data[key]);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject(); // end redactWith object

            writer.WriteEndObject(); // end rule object
        }
    }
}
