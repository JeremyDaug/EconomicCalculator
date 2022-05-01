using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Utilities;

namespace EconomicSim.Objects.Jobs;

internal class JobJsonConverter: JsonConverter<Job>
{
    public override Job? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new Job();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return result;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string propName = reader.GetString();
            
            reader.Read();
            switch (propName)
            {
                case "Name":
                    result.Name = reader.GetString();
                    break;
                case "VariantName":
                    result.VariantName = reader.GetString();
                    break;
                case "Labor":
                    var laborName = reader.GetString();
                    result.Labor = DataContext.Instance.Products.Single(x => x.GetName() == laborName);
                    break;
                case "Skill":
                    var skillName = reader.GetString();
                    result.Skill = DataContext.Instance.Skills.Single(x => x.Name == skillName);
                    break;
                case "Processes":
                    var procNames = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                    foreach (var proc in procNames)
                        result.Processes.Add(DataContext.Instance.Processes
                            .Single(
                                x => x.GetName() == proc)
                        );
                    break;
                default:
                    throw new JsonException($"Property \"{propName}\" is not valid for a Job.");
            }
        }
        
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Job value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        // name
        writer.WriteString(nameof(value.Name), value.Name);
        // variant name
        writer.WriteString(nameof(value.VariantName), value.VariantName);
        // labor
        writer.WriteString(nameof(value.Labor), value.Labor.GetName());
        // skill
        writer.WriteString(nameof(value.Skill), value.Skill.Name);
        // processes
        var procNames = value.Processes.Select(x => x.GetName());
        writer.WritePropertyName(nameof(value.Processes));
        JsonSerializer.Serialize(procNames, options);
        
        writer.WriteEndObject();
    }
}