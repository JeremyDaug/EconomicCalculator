using System.Text.Json;
using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes.ProcessTags;

namespace EconomicSim.Objects.Processes;

internal class ProcessJsonConverter : JsonConverter<Process>
{
    public override Process? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new Process();

        while (reader.Read())
        {
            // check for object end.
            if (reader.TokenType == JsonTokenType.EndObject)
                return result;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            // get property
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
                case "MinimumTime":
                    result.MinimumTime = reader.GetDecimal();
                    break;
                case "Skill":
                    var skillName = reader.GetString();
                    result.Skill = DataContext.Instance.Skills[skillName];
                    break;
                case "SkillMinimum":
                    result.SkillMinimum = reader.GetDecimal();
                    break;
                case "SkillMaximum":
                    result.SkillMaximum = reader.GetDecimal();
                    break;
                case "Description":
                    result.Description = reader.GetString();
                    break;
                case "Icon":
                    result.Icon = reader.GetString();
                    break;
                case "TechRequirement":
                    var techName = reader.GetString();
                    result.TechRequirement = DataContext.Instance.Technologies[techName];
                    break;
                case "Products":
                    if (reader.TokenType != JsonTokenType.StartObject)
                        throw new JsonException();
                    reader.Read();
                    // inputs
                    if (reader.GetString() == "Inputs")
                    {
                        reader.Read();
                        var inputs = JsonSerializer
                            .Deserialize<List<ProcessProduct>>(ref reader, options);
                        // set as input
                        foreach (var input in inputs)
                        {
                            input.Part = ProcessPartTag.Input;
                            result.ProcessProducts.Add(input);
                        }
                    }
                    reader.Read();
                    // capital
                    if (reader.GetString() == "Capital")
                    {
                        reader.Read();
                        var capital = JsonSerializer
                            .Deserialize<List<ProcessProduct>>(ref reader, options);
                        // set as capital
                        foreach (var cap in capital)
                        {
                            cap.Part = ProcessPartTag.Capital;
                            result.ProcessProducts.Add(cap);
                        }
                    }
                    reader.Read();
                    // outputs
                    if (reader.GetString() == "Outputs")
                    {
                        reader.Read();
                        var outputs = JsonSerializer
                            .Deserialize<List<ProcessProduct>>(ref reader, options);
                        // set as capital
                        foreach (var cap in outputs)
                        {
                            cap.Part = ProcessPartTag.Output;
                            result.ProcessProducts.Add(cap);
                        }
                    }

                    reader.Read();
                    break;
                case "Wants":
                    if (reader.TokenType != JsonTokenType.StartObject)
                        throw new JsonException();
                    reader.Read();
                    // inputs
                    if (reader.GetString() == "Inputs")
                    {
                        reader.Read();
                        var inputs = JsonSerializer
                            .Deserialize<List<ProcessWant>>(ref reader, options);
                        // set as input
                        foreach (var input in inputs)
                        {
                            input.Part = ProcessPartTag.Input;
                            result.ProcessWants.Add(input);
                        }
                    }
                    reader.Read();
                    // capital
                    if (reader.GetString() == "Capital")
                    {
                        reader.Read();
                        var capital = JsonSerializer
                            .Deserialize<List<ProcessWant>>(ref reader, options);
                        // set as capital
                        foreach (var cap in capital)
                        {
                            cap.Part = ProcessPartTag.Capital;
                            result.ProcessWants.Add(cap);
                        }
                    }
                    reader.Read();
                    // outputs
                    if (reader.GetString() == "Outputs")
                    {
                        reader.Read();
                        var outputs = JsonSerializer
                            .Deserialize<List<ProcessWant>>(ref reader, options);
                        // set as capital
                        foreach (var cap in outputs)
                        {
                            cap.Part = ProcessPartTag.Output;
                            result.ProcessWants.Add(cap);
                        }
                    }
                    reader.Read();
                    break;
                case "ProcessTags":
                    var tags = JsonSerializer
                        .Deserialize<List<string>>(ref reader, options);
                    foreach (var tag in tags)
                        result.ProcessTags.Add((ProcessTag)Enum.Parse(typeof(ProcessTag), tag));
                    break;
                default:
                    throw new JsonException($"Property \"{propName}\", does not exist in Process.");
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Process value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        // name
        writer.WriteString(nameof(value.Name), value.Name);
        // variant name
        writer.WriteString(nameof(value.VariantName), value.VariantName);
        // minimum time
        writer.WriteNumber(nameof(value.MinimumTime), value.MinimumTime);
        // skill
        if (value.Skill != null)
            writer.WriteString(nameof(value.Skill), value.Skill.Name);
        // skill minimum
        writer.WriteNumber(nameof(value.SkillMinimum), value.SkillMinimum);
        // skill maximum
        writer.WriteNumber(nameof(value.SkillMaximum), value.SkillMaximum);
        // Tags
        writer.WritePropertyName(nameof(ProcessTags));
        JsonSerializer.Serialize(writer, 
            value.ProcessTags.Select(x => x.ToString()));
        // description
        writer.WriteString(nameof(value.Description), value.Description);
        // icon
        writer.WriteString(nameof(value.Icon), value.Icon);
        // tech requirement
        if (value.TechRequirement != null)
            writer.WriteString(nameof(value.TechRequirement), value.TechRequirement.Name);
        
        // products
        writer.WritePropertyName("Products");
        writer.WriteStartObject();
        // input products
        writer.WritePropertyName("Inputs");
        JsonSerializer.Serialize(writer, value.InputProducts);
        // capital products
        writer.WritePropertyName("Capital");
        JsonSerializer.Serialize(writer, value.CapitalProducts);
        // output products
        writer.WritePropertyName("Outputs");
        JsonSerializer.Serialize(writer, value.OutputProducts);
        writer.WriteEndObject();
        
        // wants
        writer.WritePropertyName("Wants");
        writer.WriteStartObject();
        // input wants
        writer.WritePropertyName("Inputs");
        JsonSerializer.Serialize(writer, value.InputWants);
        // capital wants
        writer.WritePropertyName("Capital");
        JsonSerializer.Serialize(writer, value.CapitalWants);
        // output wants
        writer.WritePropertyName("Outputs");
        JsonSerializer.Serialize(writer, value.OutputWants);
        writer.WriteEndObject();
        
        writer.WriteEndObject();
    }
}