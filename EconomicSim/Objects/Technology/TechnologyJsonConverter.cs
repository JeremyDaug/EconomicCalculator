using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicSim.Objects.Technology
{
    internal class TechnologyJsonConverter : JsonConverter<Technology>
    {
        public override Technology Read(ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var result = new Technology();

            while (reader.Read())
            {
                // check for end of object
                if (reader.TokenType == JsonTokenType.EndObject)
                    return result;

                // get propertyName
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException();

                string propName = reader.GetString();

                // get value
                reader.Read();
                switch (propName)
                {
                    case "Name":
                        string value = reader.GetString();
                        result.Name = value;
                        break;
                    case "Category":
                        string cat = reader.GetString();
                        result.Category = (TechCategory)Enum.Parse(typeof(TechCategory), cat);
                        break;
                    case "Tier":
                        var tier = reader.GetInt32();
                        result.Tier = tier;
                        break;
                    case "TechCostBase":
                        var baseCost = reader.GetInt32();
                        result.TechCostBase = baseCost;
                        break;
                    case "Description":
                        var description = reader.GetString();
                        result.Description = description;
                        break;
                    case "Families":
                        var families = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                        foreach (var fam in families)
                            result.Families.Add(new TechFamily { Name = fam });
                        break;
                    case "Children":
                        var children = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                        foreach (var child in children)
                            result.Children.Add(new Technology { Name = child });
                        break;
                    case "Parents":
                        var parents = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                        foreach (var parent in parents)
                            result.Parents.Add(new Technology { Name = parent });
                        break;
                    default:
                        throw new JsonException($"Property named {propName} does not exist in Technology class.");
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer,
            Technology value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // write name
            writer.WritePropertyName(nameof(value.Name));
            JsonSerializer.Serialize(writer, value.Name, options);

            // category
            writer.WritePropertyName(nameof(value.Category));
            JsonSerializer.Serialize(writer, value.Category, options);

            // tier
            writer.WritePropertyName(nameof(value.Tier));
            JsonSerializer.Serialize(writer, value.Tier, options);

            // tech base cost
            writer.WritePropertyName(nameof(value.TechCostBase));
            JsonSerializer.Serialize(writer, value.TechCostBase, options);

            // description
            writer.WritePropertyName(nameof(value.Description));
            JsonSerializer.Serialize(writer, value.Description, options);

            // families
            writer.WritePropertyName(nameof(value.Families));
            JsonSerializer.Serialize(writer, value.Families.Select(x => x.Name), options);

            // parents
            writer.WritePropertyName(nameof(value.Parents));
            JsonSerializer.Serialize(writer, value.Parents.Select(x => x.Name), options);

            // children
            writer.WritePropertyName(nameof(value.Children));
            JsonSerializer.Serialize(writer, value.Children.Select(x => x.Name), options);

            writer.WriteEndObject();
        }
    }
}
