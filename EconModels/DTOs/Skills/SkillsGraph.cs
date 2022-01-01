using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconDTOs.DTOs.Skills
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
        public IDictionary<int, ISkill> Skills => Manager.Instance.Skills;

        /// <summary>
        /// The existing Skill Groups
        /// </summary>
        public IDictionary<int, ISkillGroup> SkillGroups => Manager.Instance.SkillGroups;

        // TODO navigation functions and the like.
    }
}
