using System.Text.Json.Serialization;

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
