using System.Text.Json.Serialization;

namespace EconomicSim.DTOs.Territory
{
    public interface ITerritoryResource
    {
        [JsonIgnore]
        int ResourceId { get; }

        string Resource { get; }

        decimal Amount { get; }
    }
}
