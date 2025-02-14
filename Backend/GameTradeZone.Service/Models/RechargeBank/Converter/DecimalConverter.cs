using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameTradeZone.Service.Models.RechargeBank.Converter
{
    public class DecimalConverter : JsonConverter<decimal?>
    {
        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDecimal();
            }
            if (reader.TokenType == JsonTokenType.String && decimal.TryParse(reader.GetString(), out decimal value))
            {
                return value;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteNumberValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
