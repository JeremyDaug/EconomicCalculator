using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Skills
{
    /// <summary>
    /// Read Only Interface for skills
    /// </summary>
    public interface ISkill
    {
        /// <summary>
        /// The Id of the skill
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The Skill's Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Description of the Skill
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The Skill Groups this skill belongs to.
        /// </summary>
        IReadOnlyList<ISkillGroup> Groups { get; }

        /// <summary>
        /// The skills related to this one.
        /// Should have a connection back on the other side.
        /// </summary>
        IReadOnlyList<(ISkill relation, decimal rate)> Relations { get; }

        /// <summary>
        /// The Labor which represents application of the skill.
        /// </summary>
        IProduct Labor { get; }
    }
}
