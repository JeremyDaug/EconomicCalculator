using System.Text.Json.Serialization;
using EconomicSim.Objects.Pops;

namespace EconomicSim.DTOs.Pops.Species
{
    public interface ISpeciesNeedDTO
    {
        string Product { get; }

        [JsonIgnore]
        int ProductId { get; }

        [JsonIgnore]
        DesireTier TierEnum { get; }

        string Tier { get; }

        decimal Amount { get; set; }

        // Need Tag modifiers
    }
}
