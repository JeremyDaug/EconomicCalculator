using System.Text.Json.Serialization;

namespace EconomicSim.DTOs.Pops
{
    public interface IPopCulturePortion
    {
        [JsonIgnore]
        int CultureId { get; }

        string Culture { get; }

        ulong Amount { get; }
    }
}