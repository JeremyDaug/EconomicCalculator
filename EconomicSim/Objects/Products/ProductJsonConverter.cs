using System.Text.Json;
using System.Text.Json.Serialization;
using EconomicSim.Objects.Products.ProductTags;

namespace EconomicSim.Objects.Products
{
    internal class ProductJsonConverter : JsonConverter<Product>
    {
        public override Product Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var result = new Product();

            while(reader.Read())
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
                    case "UnitName":
                        result.UnitName = reader.GetString();
                        break;
                    case "Quality":
                        result.Quality = reader.GetInt32();
                        break;
                    case "Mass":
                        result.Mass = reader.GetDecimal();
                        break;
                    case "Bulk":
                        result.Bulk = reader.GetDecimal();
                        break;
                    case "Fractional":
                        result.Fractional = reader.GetBoolean();
                        break;
                    case "Icon":
                        result.Icon = reader.GetString();
                        break;
                    case "ProductTags":
                        var productTags = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(ref reader, options);
                        foreach (var tag in productTags)
                            result.ProductTags
                                .Add((ProductTag)Enum.Parse(typeof(ProductTag), tag.Key), tag.Value);
                        break;
                    case "Wants":
                        if (reader.TokenType != JsonTokenType.StartArray)
                            throw new JsonException();
                        
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndArray)
                                break;

                            if (reader.TokenType != JsonTokenType.StartObject)
                                throw new JsonException();
                            reader.Read();

                            var want = reader.GetString();
                            reader.Read();
                            var amount = reader.GetDecimal();
                            result.Wants.Add((DataContext.Instance.Wants[want], amount));
                            reader.Read();

                            if (reader.TokenType != JsonTokenType.EndObject)
                                throw new JsonException();
                        }
                        break;
                    // do not load any processes.
                    case "TechRequirement":
                        var tech = reader.GetString();
                        result.TechRequirement = DataContext.Instance.Technologies[tech];
                        break;
                    default:
                        throw new JsonException($"Property {propName} is not recognized as property of Product.");
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Product value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // Name
            writer.WritePropertyName(nameof(value.Name));
            JsonSerializer.Serialize(writer, value.Name, options);

            // VariantName
            writer.WritePropertyName(nameof(value.VariantName));
            JsonSerializer.Serialize(writer, value.VariantName, options);

            // UnitName
            writer.WritePropertyName(nameof(value.UnitName));
            JsonSerializer.Serialize(writer, value.UnitName, options);

            // Quality
            writer.WritePropertyName(nameof(value.Quality));
            JsonSerializer.Serialize(writer, value.Quality, options);

            // Mass
            writer.WritePropertyName(nameof(value.Mass));
            JsonSerializer.Serialize(writer, value.Mass, options);

            // Bulk
            writer.WritePropertyName(nameof(value.Bulk));
            JsonSerializer.Serialize(writer, value.Bulk, options);

            // Fractional
            writer.WritePropertyName(nameof(value.Fractional));
            JsonSerializer.Serialize(writer, value.Fractional, options);

            // Icon
            writer.WritePropertyName(nameof(value.Icon));
            JsonSerializer.Serialize(writer, value.Icon, options);

            // ProductTags
            writer.WritePropertyName(nameof(value.ProductTags));
            JsonSerializer.Serialize(writer, value.ProductTags, options);

            // Wants
            writer.WritePropertyName(nameof(value.Wants));
            writer.WriteStartArray();
            foreach (var item in value.Wants)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(item.want.Name);
                JsonSerializer.Serialize(writer, item.amount, options);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            // TechRequirements
            if (value.TechRequirement != null)
            {
                writer.WritePropertyName(nameof(value.TechRequirement));
                JsonSerializer.Serialize(writer, value.TechRequirement.Name, options);
            }

            writer.WriteEndObject();
        }
    }
}
