using System.Text.Json;
using System.Text.Json.Serialization;

namespace EconomicSim.Objects.Firms;

internal class FirmJsonConverter : JsonConverter<Firm>
{
    public override Firm? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new Firm();

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
                case nameof(result.FirmRank):
                    result.FirmRank = (FirmRank) Enum.Parse(typeof(FirmRank), reader.GetString());
                    break;
                case nameof(result.OwnershipStructure):
                    result.OwnershipStructure = (OwnershipStructure) Enum.Parse(typeof(OwnershipStructure), reader.GetString());
                    break;
                case nameof(result.ProfitStructure):
                    result.ProfitStructure = (ProfitStructure) Enum.Parse(typeof(ProfitStructure), reader.GetString());
                    break;
                case nameof(result.Parent):
                    result.Parent = new Firm { Name = reader.GetString() };
                    break;
                case nameof(result.Children):
                    var children = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                    foreach (var child in children)
                        result.Children.Add(new Firm { Name = child });
                    break;
                case nameof(result.Jobs):
                    result.Jobs = JsonSerializer.Deserialize<List<FirmJob>>(ref reader, options);
                    break;
                case nameof(result.Products):
                    var prods = JsonSerializer.Deserialize<Dictionary<string, decimal>>(ref reader, options);
                    foreach (var prod in prods)
                    {
                        var product = DataContext.Instance.Products[prod.Key];
                        result.Products.Add(product, prod.Value);
                    }
                    break;
                case nameof(result.Resources):
                    var resources = JsonSerializer.Deserialize<Dictionary<string, decimal>>(ref reader, options);
                    foreach (var prod in resources)
                    {
                        var product = DataContext.Instance.Products[prod.Key];
                        result.Resources.Add(product, prod.Value);
                    }
                    break;
                case nameof(result.HeadQuarters):
                    var HQ = reader.GetString();
                    result.HeadQuarters = DataContext.Instance.Markets[HQ];
                    break;
                case nameof(result.Regions):
                    var regions = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                    foreach (var region in regions)
                        result.Regions.Add(DataContext.Instance.Markets[region]);
                    break;
                case nameof(result.Techs):
                    if (reader.TokenType != JsonTokenType.StartObject)
                        throw new JsonException();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            break;
                        if (reader.TokenType != JsonTokenType.PropertyName)
                            throw new JsonException();
                        var tech = reader.GetString();
                        reader.Read();
                        var research = reader.GetInt32();
                        if (result.Techs.Any(x => x.tech.Name == tech))
                            throw new JsonException("Duplicate Technology recorded in Firm.");
                        result.Techs.Add((DataContext.Instance.Technologies[tech], research));
                    }
                    break;
                default:
                    throw new JsonException();
            }
        }
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Firm value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        // Name
        writer.WriteString(nameof(value.Name), value.Name);
        // Rank
        writer.WriteString(nameof(value.FirmRank), value.FirmRank.ToString());
        // Ownership
        writer.WriteString(nameof(value.OwnershipStructure), value.OwnershipStructure.ToString());
        // Profit
        writer.WriteString(nameof(value.ProfitStructure), value.ProfitStructure.ToString());
        // Parent
        if (value.Parent != null)
            writer.WriteString(nameof(value.Parent), value.Parent.Name);
        // Children
        if (value.Children.Count > 0)
        {
            writer.WritePropertyName(nameof(value.Children));
            JsonSerializer.Serialize(writer, value.Children.Select(x => x.Name));
        }
        // Jobs
        writer.WritePropertyName(nameof(value.Jobs));
        JsonSerializer.Serialize(writer, value.Jobs);
        // Products
        writer.WritePropertyName(nameof(value.Products));
        JsonSerializer.Serialize(writer,
            value.Products.ToDictionary(x => x.Key.GetName(), 
                x => x.Value));
        // Resources
        writer.WritePropertyName(nameof(value.Resources));
        JsonSerializer.Serialize(writer,
            value.Resources.ToDictionary(x => x.Key.GetName(),
                x => x.Value));
        // Headquarters
        writer.WriteString(nameof(value.HeadQuarters), value.HeadQuarters.Name);
        // Regions
        writer.WritePropertyName(nameof(value.Regions));
        JsonSerializer.Serialize(writer, value.Regions.Select(x => x.Name));
        // Technology
        writer.WritePropertyName(nameof(value.Techs));
        writer.WriteStartObject();
        foreach (var tech in value.Techs)
        {
            writer.WritePropertyName(tech.tech.Name);
            writer.WriteNumberValue(tech.research);
        }
        writer.WriteEndObject();
        
        writer.WriteEndObject();
    }
}