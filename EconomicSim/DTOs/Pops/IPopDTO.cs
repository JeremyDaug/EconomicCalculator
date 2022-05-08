using System.Text.Json.Serialization;

namespace EconomicSim.DTOs.Pops
{
    /// <summary>
    /// Interface for population groups
    /// </summary>
    public interface IPopDTO
    {
        /// <summary>
        /// The Unique ID of the population.
        /// </summary>
        [JsonIgnore]
        int Id { get; }

        /// <summary>
        /// The total size of the populatino group.
        /// </summary>
        ulong Count { get; }

        /// <summary>
        /// The Job the population does.
        /// </summary>
        [JsonIgnore]
        int JobId { get; }

        /// <summary>
        /// The name of the job the pop does.
        /// </summary>
        string Job { get; }

        /// <summary>
        /// Firm the population is attached to.
        /// </summary>
        [JsonIgnore]
        int FirmId { get; }

        /// <summary>
        /// Name of the firm.
        /// </summary>
        string Firm { get; }

        /// <summary>
        /// Home Market's Id.
        /// </summary>
        [JsonIgnore]
        int MarketId { get; }

        /// <summary>
        /// Home Market's Name
        /// </summary>
        string Market { get; }

        /// <summary>
        /// The Skill of the Pop.
        /// </summary>
        [JsonIgnore]
        int SkillId { get; }

        /// <summary>
        /// The name of the Pop's Skill.
        /// </summary>
        string Skill { get; }

        /// <summary>
        /// The population's skill level.
        /// </summary>
        decimal SkillLevel { get; }

        // population Properties.

        /// <summary>
        /// The breakdown of species in the pop.
        /// Should add to <see cref="Count"/>.
        /// </summary>
        List<IPopSpeciesPortion> SpeciesPortions { get; }

        string SpeciesString { get; }

        /// <summary>
        /// The portion of cultures in the pop.
        /// Should add to <see cref="Count"/> or less.
        /// </summary>
        List<IPopCulturePortion> CulturePortions { get; }

        string CultureString { get; }

        // Desires Placeholder, storage not needed,
        // calculated from Species and Culture.
    }
}
