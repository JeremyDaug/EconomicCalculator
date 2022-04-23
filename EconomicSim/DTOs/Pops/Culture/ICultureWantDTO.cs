using System.Text.Json.Serialization;
using EconomicSim.Objects.Pops;

namespace EconomicSim.DTOs.Pops.Culture
{
    public interface ICultureWantDTO
    {
        string Want{ get; }

        [JsonIgnore]
        int WantId { get; }

        [JsonIgnore]
        DesireTier TierEnum { get; }

        string Tier { get; }

        decimal Amount { get; set; }

        // Need Tag modifiers
    }
}