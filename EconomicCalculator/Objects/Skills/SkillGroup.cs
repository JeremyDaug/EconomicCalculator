using System.Collections.Generic;

namespace EconomicCalculator.Objects.Skills
{
    /// <summary>
    /// Skill Group Class
    /// </summary>
    internal class SkillGroup : ISkillGroup
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public SkillGroup()
        {
            Skills = new List<Skill>();
        }

        /// <summary>
        /// Skill Group's Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Name of the Skill Group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the Skill Group.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Default transfer rate between skills within the group.
        /// </summary>
        public decimal Default { get; set; }

        /// <summary>
        /// The Skills within the group.
        /// </summary>
        public List<Skill> Skills { get; set; }
        IReadOnlyList<ISkill> ISkillGroup.Skills => Skills;
    }
}