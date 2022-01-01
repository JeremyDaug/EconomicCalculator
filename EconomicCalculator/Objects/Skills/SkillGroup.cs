using System.Collections.Generic;

namespace EconomicCalculator.Objects.Skills
{
    /// <summary>
    /// Skill Group Class
    /// </summary>
    internal class SkillGroup : ISkillGroup
    {
        private List<ISkill> _skills;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public SkillGroup()
        {
            _skills = new List<ISkill>();
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
        public IReadOnlyList<ISkill> Skills { get => _skills; }
    }
}