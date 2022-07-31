using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EconomicSim.Objects.Pops;

internal class PopJsonConverter : JsonConverter<PopGroup>
{
    public override PopGroup? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new PopGroup();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                // assert that species, cultures, and count are equal
                var specCount = result.Species.Sum(x => x.amount);
                var cultCount = result.Cultures.Sum(x => x.amount);
                if (specCount != result.Count ||
                    cultCount != result.Count)
                    throw new DataException($"Pop Group with Job \"{result.Job.GetName()}\" at Firm \"{result.Firm.Name}\" has a population mismatch.");
                return result;
            }
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            var propName = reader.GetString();
            reader.Read();
            switch (propName)
            {
                case nameof(result.Count):
                    result.Count = reader.GetInt32();
                    break;
                case nameof(result.Job):
                    var jobName = reader.GetString();
                    result.Job = DataContext.Instance.Jobs[jobName];
                    if (result.Firm != null)
                        ConnectPopToFirm(result);
                    break;
                case nameof(result.Firm):
                    result.Firm = DataContext.Instance
                        .Firms[reader.GetString()];
                    // connect the firm back to us if job has already been selected
                    if (result.Job != null)
                        ConnectPopToFirm(result);
                    break;
                case nameof(result.Market):
                    result.Market = DataContext.Instance
                        .Markets[reader.GetString()];
                    break;
                case nameof(result.LowerSkillLevel):
                    result.LowerSkillLevel = reader.GetDecimal();
                    break;
                case nameof(result.HigherSkillLevel):
                    result.HigherSkillLevel = reader.GetDecimal();
                    break;
                case nameof(result.Species):
                    if (reader.TokenType != JsonTokenType.StartObject)
                        throw new JsonException();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            break;
                        var speciesName = reader.GetString();
                        var species = DataContext.Instance.Species[speciesName];
                        reader.Read();
                        var amount = reader.GetInt32();
                        result.Species.Add((species, amount));
                    }
                    break;
                case nameof(result.Cultures):
                    if (reader.TokenType != JsonTokenType.StartObject)
                        throw new JsonException();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            break;
                        var cultureName = reader.GetString();
                        var culture = DataContext.Instance.Cultures[cultureName];
                        reader.Read();
                        var amount = reader.GetInt32();
                        result.Cultures.Add((culture, amount));
                    }
                    break;
                case nameof(result.Property):
                    var property = JsonSerializer.Deserialize<Dictionary<string, decimal>>(ref reader, options);
                    foreach (var prop in property)
                        result.Property[DataContext.Instance.Products[prop.Key]] = prop.Value;
                    break;
                default:
                    throw new JsonException($"Property \"{propName}\" in not a valid property for PopGroups.");
            }
        }
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, PopGroup value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        // Pop Count
        writer.WriteNumber(nameof(value.Count), value.Count);
        // Pop's Job
        writer.WriteString(nameof(value.Job), value.Job.GetName());
        // pop's Firm
        writer.WriteString(nameof(value.Firm), value.Firm.Name);
        // pop's Market
        writer.WriteString(nameof(value.Market), value.Market.Name);
        // don't need skill to be selected
        // Pop Lower Skill Level
        writer.WriteNumber(nameof(value.LowerSkillLevel), value.LowerSkillLevel);
        // Pop Higher Skill Level
        writer.WriteNumber(nameof(value.HigherSkillLevel), value.HigherSkillLevel);
        // Species
        writer.WritePropertyName(nameof(value.Species));
        writer.WriteStartObject();
        foreach (var spec in value.Species)
            writer.WriteNumber(spec.species.GetName(), spec.amount);
        writer.WriteEndObject();
        // Culture
        writer.WritePropertyName(nameof(value.Cultures));
        writer.WriteStartObject();
        foreach (var spec in value.Cultures)
            writer.WriteNumber(spec.culture.GetName(), spec.amount);
        writer.WriteEndObject();
        // Property
        writer.WritePropertyName(nameof(value.Property));
        var property = value.Property.ToDictionary(x => x.Key.GetName(),
            x => x.Value);
        JsonSerializer.Serialize(property, options);
        
        // close object.
        writer.WriteEndObject();
    }

    private void ConnectPopToFirm(PopGroup result)
    {
        result.Firm.Jobs
            .Single(x => x.Job.GetName() == result.Job.GetName())
            .Pop = result;
    }
}