using System.Text.Json;
using System.Text.Json.Serialization;
using EconomicSim.Objects;
using EconomicSim.Objects.Pops;

namespace EconomicSim.Helpers;

internal class NeedDesireJsonConverter : JsonConverter<NeedDesire>
{
    public override NeedDesire? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new NeedDesire();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return result;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            var prop = reader.GetString();
            reader.Read();
            switch (prop) 
            {
                case nameof(result.Product):
                    var productName = reader.GetString();
                    result.Product = DataContext.Instance.Products[productName];
                    break;
                case nameof(result.Tier):
                    var tier = reader.GetString();
                    result.Tier = (DesireTier) Enum.Parse(typeof(DesireTier), tier);
                    break;
                case nameof(result.Amount):
                    result.Amount = reader.GetDecimal();
                    break;
                default:
                    throw new JsonException();
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, NeedDesire value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        writer.WriteString(nameof(value.Product), value.Product.Name);
        writer.WriteString(nameof(value.Tier), value.Tier.ToString());
        writer.WriteNumber(nameof(value.Amount), value.Amount);
        
        writer.WriteEndObject();
    }
}