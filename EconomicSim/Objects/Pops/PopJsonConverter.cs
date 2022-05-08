using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using EconomicSim.Objects.Firms;

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
                case nameof(result.Job):
                    var jobName = reader.GetString();
                    result.Job = DataContext.Instance
                        .Jobs.Single(x => x.GetName() == jobName);
                    break;
                case nameof(result.Firm):
                    result.Firm = new Firm
                    {
                        Name = reader.GetString()
                    };
                    break;
                case nameof(result.Market):
                    result.Market = new Market.Market
                    {
                        Name = reader.GetString()
                    };
                    break;
                case nameof(result.SkillLevel):
                    result.SkillLevel = reader.GetDecimal();
                    break;
                case nameof(result.Species):
                    if (reader.TokenType != JsonTokenType.StartObject)
                        throw new JsonException();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            break;
                        var speciesName = reader.GetString();
                        var species = DataContext.Instance
                            .Species.Single(x => x.GetName() == speciesName);
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
                        var culture = DataContext.Instance
                            .Cultures.Single(x => x.GetName() == cultureName);
                        reader.Read();
                        var amount = reader.GetInt32();
                        result.Cultures.Add((culture, amount));
                    }
                    break;
                case nameof(result.Property):
                    if (reader.TokenType != JsonTokenType.StartObject)
                        throw new JsonException();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            break;
                        var productName = reader.GetString();
                        var product = DataContext.Instance
                            .Products.Single(x => x.GetName() == productName);
                        reader.Read();
                        var amount = reader.GetDecimal();
                        result.Property.Add((product, amount));
                    }
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
        
        // Pop's Job
        writer.WriteString(nameof(value.Job), value.Job.GetName());
        // pop's Firm
        writer.WriteString(nameof(value.Firm), value.Firm.Name);
        // pop's Market
        writer.WriteString(nameof(value.Market), value.Market.Name);
        // don't need skill to be selected
        // Pop Skill Level
        writer.WriteNumber(nameof(value.SkillLevel), value.SkillLevel);
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
        writer.WriteStartObject();
        foreach (var spec in value.Property)
            writer.WriteNumber(spec.product.GetName(), spec.amount);
        writer.WriteEndObject();
        
        writer.WriteEndObject();
    }
}