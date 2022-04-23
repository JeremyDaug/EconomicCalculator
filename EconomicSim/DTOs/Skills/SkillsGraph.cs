using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicSim.DTOs.Skills
{
    /// <summary>
    /// Storage Space for our skills, and allows for
    /// navigation between skills and skill groups.
    /// </summary>
    public class SkillsGraph
    {
        /// <summary>
        /// The Existing skills.
        /// </summary>
        public IDictionary<int, ISkillDTO> Skills => DTOManager.Instance.Skills;

        /// <summary>
        /// The existing Skill Groups
        /// </summary>
        public IDictionary<int, ISkillGroupDTO> SkillGroups => DTOManager.Instance.SkillGroups;

        // TODO navigation functions and the like.
    }
}
