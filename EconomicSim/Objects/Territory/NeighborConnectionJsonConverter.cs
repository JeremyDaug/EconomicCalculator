using System.Text.Json;
using System.Text.Json.Serialization;
using EconomicSim.Enums;

namespace EconomicSim.Objects.Territory;

internal class NeighborConnectionJsonConverter : JsonConverter<NeighborConnection>
{
    public override NeighborConnection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new NeighborConnection();
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
                case nameof(result.Neighbor):
                    result.Neighbor = new Territory {Name = reader.GetString() };
                    break;
                case nameof(result.Distance):
                    result.Distance = reader.GetDecimal();
                    break;
                case nameof(result.Type):
                    result.Type =
                        (TerritoryConnectionType) Enum.Parse(typeof(TerritoryConnectionType), reader.GetString());
                    break;
                default:
                    throw new JsonException();
            }
        }
        
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, NeighborConnection value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        // Neighbor
        writer.WriteString(nameof(value.Neighbor), value.Neighbor.Name);
        // Distance
        writer.WriteNumber(nameof(value.Distance), value.Distance);
        // Type
        writer.WriteString(nameof(value.Type), value.Type.ToString());
        
        writer.WriteEndObject();
    }
}