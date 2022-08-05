using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EconomicSim.Objects.Market;

internal class MarketJsonConverter : JsonConverter<Market>
{
    public override Market? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new Market();

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
                case nameof(result.Name):
                    result.Name = reader.GetString();
                    break;
                case nameof(result.Territories):
                    var territories = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                    result.Territories = territories
                        .Select(x => DataContext.Instance.Territories[x]).ToList();
                    foreach (var terr in result.Territories)
                    {
                        if (terr.Market != null)
                            throw new DataException($"Multiple Markets claim ownership on Territory \"{terr.Name}\".");
                        terr.Market = result;
                    }
                    break;
                case nameof(result.Resources):
                    var resources = JsonSerializer.Deserialize<Dictionary<string, decimal>>(ref reader, options);
                    result.Resources = resources
                        .ToDictionary(x => DataContext.Instance.Products[x.Key],
                            x => x.Value);
                    break;
                case nameof(result.PaymentPreference):
                    var preferences = JsonSerializer
                        .Deserialize<Dictionary<string, decimal>>(ref reader, options);
                    foreach (var preference in preferences)
                        result.PaymentPreference
                            .Add(DataContext.Instance.Products[preference.Key],
                                preference.Value);
                    break;
                default:
                    throw new JsonException($"");
            }
        }
        
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Market value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        // Name
        writer.WriteString(nameof(value.Name), value.Name);
        // Skip Firms
        // Skip Pops
        // Governor Placeholder?
        // Territories
        writer.WritePropertyName(nameof(value.Territories));
        JsonSerializer.Serialize(writer, value.Territories.Select(x => x.Name));
        // Skipped Neighbors, based on Territories
        // Resources
        writer.WritePropertyName(nameof(value.Resources));
        writer.WriteStartObject();
        foreach (var resource in value.Resources)
            writer.WriteNumber(resource.Key.GetName(), resource.Value);
        writer.WriteEndObject();
        // Payment Preferences
        if (value.PaymentPreference.Any())
        {
            writer.WritePropertyName(nameof(value.PaymentPreference));
            var temp = value.PaymentPreference
                .ToDictionary(
                    x => x.Key.GetName(),
                    x => x.Value);
            JsonSerializer.Serialize(writer, temp);
        }
        
        writer.WriteEndObject();
    }
}