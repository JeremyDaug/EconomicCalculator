using System.Text.Json;
using System.Text.Json.Serialization;
using EconomicSim.Helpers;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Pops.Species;

internal class SpeciesJsonConverter : JsonConverter<Species>
{
    public override Species? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new Species();

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
                case "Name":
                    result.Name = reader.GetString();
                    break;
                case "VariantName":
                    result.VariantName = reader.GetString();
                    break;
                case "GrowthRate":
                    result.GrowthRate = reader.GetDecimal();
                    break;
                case "DeathRate":
                    result.DeathRate = reader.GetDecimal();
                    break;
                case "Needs":
                    var needs = JsonSerializer.Deserialize<List<NeedDesire>>(ref reader, options);
                    result.Needs = needs;
                    break;
                case "Wants":
                    var wants = JsonSerializer.Deserialize<List<WantDesire>>(ref reader, options);
                    result.Wants = wants;
                    break;
                case "Tags":
                    var tags
                        = JsonSerializer.Deserialize<List<TagData<SpeciesTag>>>(ref reader,
                            new JsonSerializerOptions
                            {
                                Converters = { new TagDataJsonConverter<SpeciesTag>() }
                            });
                    foreach (var tag in tags)
                        result.Tags.Add(tag);
                    break;
                case "Relations":
                    var relNames = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                    foreach (var name in relNames)
                        result.Relations.Add(new Species{Name = name});
                    break;
                default: 
                    throw new JsonException($"Property \"{propName}\" is not a valid property of Species.");
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Species value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        // name
        writer.WriteString(nameof(value.Name), value.Name);
        // variant name
        if (!string.IsNullOrWhiteSpace(value.VariantName))
            writer.WriteString(nameof(value.VariantName), value.VariantName);
        // growth rate
        writer.WriteNumber(nameof(value.GrowthRate), value.GrowthRate);
        // death rate
        writer.WriteNumber(nameof(value.DeathRate), value.DeathRate);
        // Needs
        writer.WritePropertyName(nameof(value.Needs));
        JsonSerializer.Serialize(writer, value.Needs, options);
        // Wants
        writer.WritePropertyName(nameof(value.Wants));
        JsonSerializer.Serialize(writer, value.Wants, options);
        // Tags
        writer.WritePropertyName(nameof(value.Tags));
        var option = new JsonSerializerOptions(options);
        option.Converters.Add(new TagDataJsonConverter<SpeciesTag>());
        JsonSerializer.Serialize(writer, value.Tags, option);
        // Relations
        writer.WritePropertyName(nameof(value.Relations));
        JsonSerializer.Serialize(writer, value.Relations, options);
        
        writer.WriteEndObject();
    }
}