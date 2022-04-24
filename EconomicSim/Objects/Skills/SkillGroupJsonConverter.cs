using System.Text.Json;
using System.Text.Json.Serialization;

namespace EconomicSim.Objects.Skills;

internal class SkillGroupJsonConverter: JsonConverter<SkillGroup>
{
    public override SkillGroup Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new SkillGroup();

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
                case "Description":
                    result.Description = reader.GetString();
                    break;
                case "Default":
                    result.Default = reader.GetDecimal();
                    break;
                case "Skills":
                    List<string> skills = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                    // get the skills that have loaded and connect them back.
                    foreach (var skill in skills)
                    {
                        var child = DataContext.Instance.Skills.Single(x => x.Name == skill);
                        result.Skills.Add(child);
                        child.Groups.Add(result);
                    }
                    break;
                default:
                    throw new JsonException($"Property {propName} does not exist in Skill Group.");
            }
        }
        
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, SkillGroup value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        // name
        writer.WriteString(nameof(value.Name), value.Name);
        
        // description
        writer.WriteString(nameof(value.Description), value.Description);
        // default transfer rate
        writer.WriteNumber(nameof(value.Default), value.Default);
        // Skills
        writer.WritePropertyName(nameof(value.Skills));
        JsonSerializer.Serialize(writer, value.Skills.Select(x => x.Name), options);
        
        writer.WriteEndObject();
    }
}