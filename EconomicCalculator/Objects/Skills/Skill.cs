using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Skills
{
    /// <summary>
    /// Skill Data Class
    /// </summary>
    internal class Skill : ISkill
    {
        private List<Tuple<ISkill, decimal>> _relations;
        private List<ISkillGroup> _groups;

        public Skill()
        {
            _relations = new List<Tuple<ISkill, decimal>>();
            _groups = new List<ISkillGroup>();
        }

        /// <summary>
        /// Skill Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Name of the Skill.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the Skill.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Groups this skill is in.
        /// </summary>
        public IReadOnlyList<ISkillGroup> Groups { get => _groups; }

        /// <summary>
        /// The other skills related to this skill.
        /// </summary>
        public IReadOnlyList<Tuple<ISkill, decimal>> Relations { get => _relations; }
    }
}
