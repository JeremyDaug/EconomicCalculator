using System.Text.Json.Serialization;

namespace EconomicSim.DTOs.Pops
{
    public class PopDTO : IPopDTO
    {
        public PopDTO()
        {
            SpeciesPortions = new List<IPopSpeciesPortion>();
            CulturePortions = new List<IPopCulturePortion>();
        }

        /// <summary>
        /// The Unique ID of the population.
        /// </summary>
        [JsonIgnore] 
        public int Id { get; set; }

        /// <summary>
        /// The total size of the populatino group.
        /// </summary>
        public ulong Count { get; set; }

        /// <summary>
        /// The Job the population does.
        /// </summary>
        [JsonIgnore]
        public int JobId { get; set; }

        /// <summary>
        /// The name of the job the pop does.
        /// </summary>
        public string Job { get; set; }

        /// <summary>
        /// Firm the population is attached to.
        /// </summary>
        [JsonIgnore]
        public int FirmId { get; set; }

        /// <summary>
        /// Name of the firm.
        /// </summary>
        public string Firm { get; set; }

        /// <summary>
        /// Home Market's Id.
        /// </summary>
        [JsonIgnore]
        public int MarketId { get; set; }

        /// <summary>
        /// Home Market's Name
        /// </summary>
        public string Market { get; set; }

        /// <summary>
        /// The Skill of the Pop.
        /// </summary>
        [JsonIgnore]
        public int SkillId { get; set; }

        /// <summary>
        /// The name of the Pop's Skill.
        /// </summary>
        public string Skill { get; set; }

        /// <summary>
        /// The population's skill level.
        /// </summary>
        public decimal SkillLevel { get; set; }

        // population Properties.

        // dependants

        /// <summary>
        /// The breakdown of species in the pop.
        /// Should add to <see cref="Count"/>.
        /// </summary>
        public List<IPopSpeciesPortion> SpeciesPortions { get; set; }

        public string SpeciesString
        {
            get
            {
                var result = "";
                foreach (var p in SpeciesPortions)
                {
                    result = p.ToString() + ";\n";
                }
                return result;
            }
        }

        /// <summary>
        /// The portion of cultures in the pop.
        /// Should add to <see cref="Count"/> or less.
        /// </summary>
        public List<IPopCulturePortion> CulturePortions { get; set; }

        public string CultureString
        {
            get
            {
                var result = "";
                foreach (var p in CulturePortions)
                {
                    result = p.ToString() + ";\n";
                }
                return result;
            }
        }

        // ideology

        // politics

        // Desires Placeholder, storage not needed,
        // calculated from Species and Culture.

        public override string ToString()
        {
            return string.Format("{0}'s {1} of {2}", Market, Job, Firm);
        }
    }
}
