using System.Text.Json;
using System.Text.Json.Serialization;
using EconomicSim.Objects;
using EconomicSim.Objects.Pops;

namespace EconomicSim.Helpers;

internal class WantDesireJsonConverter : JsonConverter<WantDesire>
{
    public override WantDesire? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new WantDesire();

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
                case nameof(result.Want):
                    var wantName = reader.GetString();
                    result.Want = DataContext.Instance.Wants.Single(x => x.Name == wantName);
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

    public override void Write(Utf8JsonWriter writer, WantDesire value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        writer.WriteString(nameof(value.Want), value.Want.Name);
        writer.WriteString(nameof(value.Tier), value.Tier.ToString());
        writer.WriteNumber(nameof(value.Amount), value.Amount);
        
        writer.WriteEndObject();
    }
}