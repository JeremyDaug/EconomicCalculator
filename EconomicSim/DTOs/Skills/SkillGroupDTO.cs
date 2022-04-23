using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicSim.DTOs.Skills
{
    /// <summary>
    /// Skill Group Information
    /// </summary>
    public class SkillGroupDTO : ISkillGroupDTO
    {
        /// <summary>
        /// The Id of the skill Group
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// The name of the skill group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Default transfer rate between any two Skills
        /// within this skill group.
        /// </summary>
        public decimal Default { get; set; }

        /// <summary>
        /// The Description of the Skill Group.
        /// </summary>
        public string Description { get; set; }
    }
}
