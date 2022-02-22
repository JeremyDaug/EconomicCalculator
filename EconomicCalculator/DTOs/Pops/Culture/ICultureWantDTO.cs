using EconomicCalculator.Objects.Pops;
using System.Text.Json.Serialization;

namespace EconomicCalculator.DTOs.Pops.Culture
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