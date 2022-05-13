using System.Text.Json;
using System.Text.Json.Serialization;

namespace EconomicSim.Objects.Skills
{
    internal class SkillJsonConverter : JsonConverter<Skill>
    {
        public override Skill Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var result = new Skill();

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
                    case "Description":
                        result.Description = reader.GetString();
                        break;
                    case "Relations":
                        if (reader.TokenType != JsonTokenType.StartObject)
                            throw new JsonException();

                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndObject)
                                break;

                            var rel = reader.GetString();
                            reader.Read();
                            var rate = reader.GetDecimal();

                            var skill = new Skill { Name = rel };
                            result.Relations.Add((skill, rate));
                        }
                        break;
                    case "Labor":
                        var prodName = reader.GetString();
                        result.Labor = DataContext.Instance.Products[prodName];
                        break;
                    default:
                        throw new JsonException($"Property {propName} is not valid for a skill.");
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Skill value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // name
            writer.WritePropertyName(nameof(value.Name));
            JsonSerializer.Serialize(writer, value.Name, options);

            // description
            writer.WritePropertyName(nameof(value.Description));
            JsonSerializer.Serialize(writer, value.Description, options);

            // skip groups
            // relations
            writer.WritePropertyName(nameof(value.Relations));
            writer.WriteStartObject();
            foreach (var item in value.Relations)
            {
                writer.WritePropertyName(item.relation.Name);
                JsonSerializer.Serialize(writer, item.rate, options);
            }
            writer.WriteEndObject();

            // labor
            writer.WritePropertyName(nameof(value.Labor));
            JsonSerializer.Serialize(writer, value.Labor.Name, options);

            writer.WriteEndObject();
        }
    }
}
