using System.Text.Json;
using System.Text.Json.Serialization;

namespace EconomicSim.Helpers;

internal class TagDataJsonConverter<T> : JsonConverter<TagData<T>> where T : Enum
{
    public override TagData<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new TagData<T>();
        reader.Read();

        // get tag
        result.Tag = (T) Enum.Parse(typeof(T), reader.GetString());

        reader.Read();
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        while (reader.Read())
        { // get Parameters
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            var key = reader.GetString();
            reader.Read();
            var value = reader.GetString();
            result.Parameters.Add(key, value);
        }

        reader.Read();
        return result;
    }

    public override void Write(Utf8JsonWriter writer, TagData<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        // tag
        writer.WritePropertyName(value.Tag.ToString());
        // data
        writer.WriteStartObject();
        foreach (var param in value.Parameters)
        {
            writer.WriteString(param.Key, param.Value.ToString());
        }
        writer.WriteEndObject();
        
        // close
        writer.WriteEndObject();
    }
}