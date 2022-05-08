using System.Text.Json;
using System.Text.Json.Serialization;

namespace EconomicSim.DTOs
{
    public class AbstractConverter<TReal, TAbstract>
        : JsonConverter<TAbstract> where TReal : TAbstract
    {
        public override TAbstract Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<TReal>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, TAbstract value, JsonSerializerOptions options) { }
    }
}
