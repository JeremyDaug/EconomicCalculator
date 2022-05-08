using System.Text.Json.Serialization;

namespace EconomicSim.DTOs.Territory
{
    public interface IResourceNode
    {
        [JsonIgnore]
        int ResourceId { get; }

        string Resource { get; }

        decimal Stockpile { get; }

        int Depth { get; }
    }
}
