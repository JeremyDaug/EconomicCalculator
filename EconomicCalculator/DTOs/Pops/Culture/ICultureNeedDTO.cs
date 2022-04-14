using EconomicCalculator.Objects.Pops;
using System.Text.Json.Serialization;

namespace EconomicCalculator.DTOs.Pops.Culture
{
    public interface ICultureNeedDTO
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