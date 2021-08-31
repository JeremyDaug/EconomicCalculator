using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Skills
{
    /// <summary>
    /// Skill interface.
    /// </summary>
    public interface ISkill
    {
        /// <summary>
        /// Skill Id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Skill's Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The groups the skill belongs to.
        /// </summary>
        List<string> GroupStrings { get; }

        /// <summary>
        /// The Groups the skill Belongs to.
        /// </summary>
        [JsonIgnore]
        List<int> Groups { get; }

        /// <summary>
        /// The Groups it belongs to in string form.
        /// </summary>
        [JsonIgnore]
        string GroupString { get; }

        /// <summary>
        /// Skills that this skill is related to.
        /// The key is the Id of the skill
        /// The value is the transfer rate between this skill
        /// and the other.
        /// </summary>
        [JsonIgnore]
        Dictionary<int, decimal> Related { get; }

        /// <summary>
        /// Skill Relations as strings
        /// </summary>
        Dictionary<string, decimal> RelatedStrings { get; }

        /// <summary>
        /// The Related Items in singular string from.
        /// </summary>
        [JsonIgnore]
        string RelatedString { get; }

        /// <summary>
        /// The Description of the skill.
        /// </summary>
        string Description { get; }
    }
}
