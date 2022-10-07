using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using EconomicSim.Objects;

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
            {
                if (result.IsStretched && result.StartTier > result.EndTier)
                    throw new DataException("Desire Tier must be before its stop.");
                if (result.EndTier.HasValue && result.Step == 0)
                    throw new DataException("Desire with Stop must have a Positive Step Value.");
                return result;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            var prop = reader.GetString();
            reader.Read();
            switch (prop) 
            {
                case nameof(result.Want):
                    var wantName = reader.GetString();
                    result.Want = DataContext.Instance.Wants[wantName];
                    break;
                case nameof(result.StartTier):
                    result.StartTier = reader.GetInt32();
                    break;
                case nameof(result.Amount):
                    result.Amount = reader.GetDecimal();
                    break;
                case nameof(result.Step):
                    var step = reader.GetInt32();
                    if (step < 0)
                        throw new JsonException($"NeedDesire Step must be non-negative value.");
                    result.Step = step;
                    break;
                case nameof(result.EndTier):
                    result.EndTier = reader.GetInt32();
                    break;
                case "Tier": // TODO remove these two later. Transitory code.
                    result.StartTier = reader.GetInt32();
                    break;
                case "Stop":
                    result.EndTier = reader.GetInt32();
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
        writer.WriteString(nameof(value.StartTier), value.StartTier.ToString());
        if (value.Step > 0)
            writer.WriteNumber(nameof(value.Step), value.Step);
        if (value.EndTier.HasValue)
            writer.WriteNumber(nameof(value.EndTier), value.EndTier.Value);
        writer.WriteNumber(nameof(value.Amount), value.Amount);
        
        writer.WriteEndObject();
    }
}