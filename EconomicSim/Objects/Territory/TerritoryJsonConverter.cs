using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Utilities;
using EconomicSim.Enums;
using EconomicSim.Objects.Pops;

namespace EconomicSim.Objects.Territory;

internal class TerritoryJsonConverter: JsonConverter<Territory>
{
    public override Territory? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new Territory();

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
                case nameof(result.Coastal):
                    result.Coastal = reader.GetBoolean();
                    break;
                case nameof(result.Lake):
                    result.Lake = reader.GetBoolean();
                    break;
                case nameof(result.Size):
                    result.Size = reader.GetUInt64();
                    break;
                case nameof(result.Land):
                    result.Land = reader.GetUInt64();
                    break;
                case nameof(result.Plots):
                    var plots = JsonSerializer.Deserialize<Dictionary<string, long>>(ref reader, options);
                    var itemedPlots = plots.ToDictionary(
                        x => DataContext.Instance.Products.Single(y => y.GetName() == x.Key),
                        x => x.Value);
                    result.Plots = itemedPlots;
                    break;
                case nameof(result.Neighbors):
                    var neighbors = JsonSerializer.Deserialize<List<NeighborConnection>>(ref reader, options);
                    result.Neighbors = neighbors;
                    break;
                case nameof(result.Nodes):
                    result.Nodes = JsonSerializer.Deserialize<List<Node>>(ref reader, options);
                    break;
                case nameof(result.Resources):
                    var resources = JsonSerializer.Deserialize<Dictionary<string, decimal>>(ref reader, options);
                    result.Resources =
                        resources.ToDictionary(
                            x => DataContext.Instance.Products.Single(y => y.GetName() == x.Key),
                            x => x.Value);
                    break;
                default:
                    throw new JsonException($"Property \"{propName}\" is not valid for a Territory.");
            }
        }
        
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Territory value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        // name
        writer.WriteString(nameof(value.Name), value.Name);
        // is coastal
        writer.WriteBoolean(nameof(value.Coastal), value.Coastal);
        // has lake
        writer.WriteBoolean(nameof(value.Lake), value.Lake);
        // Total size in acres
        writer.WriteNumber(nameof(value.Size), value.Size);
        // Land
        writer.WriteNumber(nameof(value.Land), value.Land);
        // Plots
        writer.WritePropertyName(nameof(value.Plots));
        var plots = value.Plots.ToDictionary(x => x.Key.GetName(),
            x => x.Value);
        JsonSerializer.Serialize(writer, plots, options);
        // Special Neighbors
        writer.WritePropertyName(nameof(value.Neighbors));
        JsonSerializer.Serialize(writer, value.Neighbors, options);
        // Nodes
        writer.WritePropertyName(nameof(value.Nodes));
        JsonSerializer.Serialize(writer, value.Nodes, options);
        // Resources
        writer.WritePropertyName(nameof(value.Resources));
        writer.WriteStartObject();
        foreach (var resource in value.Resources)
        {
            writer.WriteNumber(resource.Key.GetName(), resource.Value);
        }
        writer.WriteEndObject();
        
        writer.WriteEndObject();
    }
}