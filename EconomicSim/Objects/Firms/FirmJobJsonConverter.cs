using System.Text.Json;
using System.Text.Json.Serialization;

namespace EconomicSim.Objects.Firms;

internal class FirmJobJsonConverter : JsonConverter<FirmJob>
{
    public override FirmJob? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var result = new FirmJob();

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
                case nameof(result.Job):
                    var jobName = reader.GetString();
                    result.Job = DataContext.Instance
                        .Jobs[jobName];
                    break;
                case nameof(result.WageType):
                    result.WageType = (WageType) Enum.Parse(typeof(WageType), reader.GetString());
                    break;
                case nameof(result.Wage):
                    result.Wage = reader.GetDecimal();
                    break;
                default:
                    throw new JsonException();
            }
        }
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, FirmJob value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        writer.WriteString(nameof(value.Job), value.Job.GetName());
        writer.WriteString(nameof(value.WageType), value.WageType.ToString());
        writer.WriteNumber(nameof(value.Wage), value.Wage);
        
        writer.WriteEndObject();
    }
}