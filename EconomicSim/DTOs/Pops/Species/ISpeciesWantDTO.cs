using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicSim.Objects.Pops;

namespace EconomicSim.DTOs.Pops.Species
{
    public interface ISpeciesWantDTO
    {
        string Want { get; }

        [JsonIgnore]
        int WantId { get; }

        [JsonIgnore]
        DesireTier TierEnum { get; }

        string Tier { get; }

        decimal Amount { get; set; }

        // Need Tag modifiers
    }
}
