using System.Text.Json;
using System.Text.Json.Serialization;
using EconomicSim.Helpers;

namespace EconomicSim.Objects.Pops.Culture;

internal class CultureJsonConverter : JsonConverter<Culture>
{
    public override Culture? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new Culture();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return result;
            
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            var propName = reader.GetString();
            reader.Read();
            switch (propName)
            {
                case nameof(result.Name):
                    result.Name = reader.GetString();
                    break;
                case nameof(result.VariantName):
                    result.VariantName = reader.GetString();
                    break;
                case nameof(result.GrowthModifier):
                    result.GrowthModifier = reader.GetDecimal();
                    break;
                case nameof(result.DeathModifier):
                    result.DeathModifier = reader.GetDecimal();
                    break;
                case nameof(result.Needs):
                    var needs = JsonSerializer.Deserialize<List<NeedDesire>>(ref reader, options);
                    result.Needs = needs;
                    break;
                case nameof(result.Wants):
                    var wants = JsonSerializer.Deserialize<List<WantDesire>>(ref reader, options);
                    result.Wants = wants;
                    break;
                case nameof(result.Tags):
                    var tags = JsonSerializer.Deserialize<List<TagData<CultureTag>>>(ref reader,
                        new JsonSerializerOptions
                        {
                            Converters = {new TagDataJsonConverter<CultureTag>()}
                        });
                    foreach (var tag in tags)
                        result.Tags.Add(tag);
                    break;
                default:
                    throw new JsonException($"Property \"{propName}\" is not a valid property for a Culture.");
            }
        }
        
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Culture value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        // name
        writer.WriteString(nameof(value.Name), value.Name);
        // variant
        writer.WriteString(nameof(value.VariantName), value.VariantName);
        // growth bonus
        writer.WriteNumber(nameof(value.GrowthModifier), value.GrowthModifier);
        // death bonus
        writer.WriteNumber(nameof(value.DeathModifier), value.DeathModifier);
        // Needs
        writer.WritePropertyName(nameof(value.Needs));
        JsonSerializer.Serialize(writer, value.Needs, options);
        // Wants
        writer.WritePropertyName(nameof(value.Wants));
        JsonSerializer.Serialize(writer, value.Wants, options);
        // Tags
        writer.WritePropertyName(nameof(value.Needs));
        var newOptions = new JsonSerializerOptions(options);
        newOptions.Converters.Add(new TagDataJsonConverter<CultureTag>());
        JsonSerializer.Serialize(writer, value.Needs, newOptions);
        
        writer.WriteEndObject();
    }
}