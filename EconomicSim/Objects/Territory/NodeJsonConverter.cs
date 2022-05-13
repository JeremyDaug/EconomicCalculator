using System.Text.Json;
using System.Text.Json.Serialization;

namespace EconomicSim.Objects.Territory;

internal class NodeJsonConverter: JsonConverter<Node>
{
    public override Node? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new Node();

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
                case nameof(result.Resource):
                    var name = reader.GetString();
                    result.Resource = DataContext.Instance.Products[name];
                    break;
                case nameof(result.Stockpile):
                    result.Stockpile = reader.GetDecimal();
                    break;
                case nameof(result.Depth):
                    result.Depth = reader.GetInt32();
                    break;
            }
        }
        
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Node value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        // Resource
        writer.WriteString(nameof(value.Resource), value.Resource.GetName());
        // Stockpile
        writer.WriteNumber(nameof(value.Stockpile), value.Stockpile);
        // Depth
        writer.WriteNumber(nameof(value.Depth), value.Depth);
        
        writer.WriteEndObject();
    }
}