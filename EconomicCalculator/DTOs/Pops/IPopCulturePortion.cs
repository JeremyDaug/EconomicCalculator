using System.Text.Json.Serialization;

namespace EconomicCalculator.DTOs.Pops
{
    public interface IPopCulturePortion
    {
        [JsonIgnore]
        int CultureId { get; }

        string Culture { get; }

        ulong Amount { get; }
    }
}