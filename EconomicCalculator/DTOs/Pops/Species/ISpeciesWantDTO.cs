using EconomicCalculator.Objects.Pops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops.Species
{
    public interface ISpeciesWantDTO
    {
        string Want { get; }

        [JsonIgnore]
        int WantId { get; }

        [JsonIgnore]
        DesireTier TierEnum { get; }

        string Tier { get; }

        decimal Amount { get; }

        // Need Tag modifiers
    }
}
