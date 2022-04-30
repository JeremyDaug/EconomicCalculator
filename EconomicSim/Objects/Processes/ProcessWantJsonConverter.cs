using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes.ProductionTags;

namespace EconomicSim.Objects.Processes;

internal class ProcessWantJsonConverter : JsonConverter<ProcessWant>
{
    public override ProcessWant? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new ProcessWant();

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
                case "Want":
                    var name = reader.GetString();
                    result.Want = DataContext.Instance.Wants.Single(x => x.Name == name);
                    break;
                case "Amount":
                    result.Amount = reader.GetDecimal();
                    break; 
                case "Tags":
                    // assert is object
                    if (reader.TokenType != JsonTokenType.StartObject)
                        throw new JsonException();

                    // get into the properties
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            break; // end of list
                        // get the tag name
                        var tag = (ProductionTag)Enum.Parse(typeof(ProductionTag),reader.GetString());
                        
                        reader.Read();
                        // tag properties
                        if (reader.TokenType != JsonTokenType.StartObject)
                            throw new JsonException();
                        Dictionary<string, object> props = new Dictionary<string, object>();
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndObject)
                                break;
                            // property
                            var prop = reader.GetString();
                            reader.Read();
                            // get value TODO make this read values into the correct types.
                            var value = reader.GetString();
                            props.Add(prop, value);
                        }
                        // add data to object.
                        result.TagData.Add((tag, props));
                    }
                    break;
                default:
                    throw new JsonException($"Property \"{propName}\" does not exist in Process Product.");
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, ProcessWant value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        // Want name
        writer.WriteString(nameof(value.Want), value.Want.Name);
        
        // amount 
        writer.WriteNumber(nameof(value.Amount), value.Amount);
        
        // tags
        writer.WritePropertyName("Tags");
        writer.WriteStartObject();
        if (value.TagData.Count > 0)
        {
            foreach (var tag in value.TagData)
            {
                writer.WritePropertyName(tag.tag.ToString());
                writer.WriteStartObject();
                if (tag.properties != null)
                {
                    foreach (var prop in tag.properties)
                        writer.WriteString(prop.Key, prop.Value.ToString());
                }
                writer.WriteEndObject();
            }
        }
        writer.WriteEndObject();

        // part is set elsewhere.
        
        writer.WriteEndObject();
    }
}