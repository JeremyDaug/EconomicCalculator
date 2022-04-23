using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicSim.Objects.Technology
{
    internal class TechFamilyJsonConverter : JsonConverter<TechFamily>
    {
        public override TechFamily Read(ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var result = new TechFamily();

            while (reader.Read())
            {
                // check for the end of the object
                if (reader.TokenType == JsonTokenType.EndObject)
                    return result;

                // get the property name
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
                    case "Relations":
                        List<string> rels = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                        foreach (var rel in rels)
                            result.Relations.Add(new TechFamily { Name = rel });
                        break;
                    case "Description":
                        string desc = reader.GetString();
                        result.Description = desc;
                        break;
                    default:
                        throw new JsonException($"Property named {propName} does not exist in Tech Family class.");
                }
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, TechFamily value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // write name
            writer.WritePropertyName(nameof(value.Name));
            JsonSerializer.Serialize(writer, value.Name, options);

            // write relations
            writer.WritePropertyName(nameof(value.Relations));
            JsonSerializer.Serialize(writer, value.Relations.Select(x => x.Name), options);

            // ignore techs

            // write Descriptions
            writer.WritePropertyName(nameof(value.Description));
            JsonSerializer.Serialize(writer, value.Description, options);

            writer.WriteEndObject();
        }
    }
}
