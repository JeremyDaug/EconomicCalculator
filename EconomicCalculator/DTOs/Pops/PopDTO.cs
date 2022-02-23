using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops
{
    public class PopDTO : IPopDTO
    {
        /// <summary>
        /// The Unique ID of the population.
        /// </summary>
        [JsonIgnore] 
        public int Id { get; }

        /// <summary>
        /// The total size of the populatino group.
        /// </summary>
        public ulong Count { get; }

        /// <summary>
        /// The Job the population does.
        /// </summary>
        [JsonIgnore]
        public int JobId { get; }

        /// <summary>
        /// The name of the job the pop does.
        /// </summary>
        public string Job { get; }

        /// <summary>
        /// Firm the population is attached to.
        /// </summary>
        [JsonIgnore]
        public int FirmId { get; }

        /// <summary>
        /// Name of the firm.
        /// </summary>
        public string Firm { get; }

        /// <summary>
        /// Home Market's Id.
        /// </summary>
        [JsonIgnore]
        public int MarketId { get; }

        /// <summary>
        /// Home Market's Name
        /// </summary>
        public string Market { get; }

        /// <summary>
        /// The Skill of the Pop.
        /// </summary>
        [JsonIgnore]
        public int Skill { get; }

        /// <summary>
        /// The name of the Pop's Skill.
        /// </summary>
        public string SkillName { get; }

        // population Properties.

        /// <summary>
        /// The breakdown of species in the pop.
        /// Should add to <see cref="Count"/>.
        /// </summary>
        public List<IPopSpeciesPortion> SpeciesPortions { get; }

        /// <summary>
        /// The portion of cultures in the pop.
        /// Should add to <see cref="Count"/> or less.
        /// </summary>
        public List<IPopCulturePortion> CulturePortions { get; }

        // Desires Placeholder, storage not needed,
        // calculated from Species and Culture.

        public override string ToString()
        {
            return string.Format("{0}'s {1} of {2}", Market, Job, Firm);
        }
    }
}
