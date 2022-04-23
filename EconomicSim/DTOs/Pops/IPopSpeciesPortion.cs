using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicSim.DTOs.Pops
{
    public interface IPopSpeciesPortion
    {
        /// <summary>
        /// The Id of the species
        /// </summary>
        [JsonIgnore]
        int SpeciesId { get; }

        /// <summary>
        /// The Species of the Pop Portion
        /// </summary>
        string Species { get; }

        /// <summary>
        /// The number of members of this species
        /// </summary>
        ulong Amount { get; }
    }
}
