using EconomicCalculator.Storage.Processes.ProcessTags;
using EconomicCalculator.Storage.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Processes
{

    public interface IProcess
    {
        int Id { get; }

        string Name { get; }

        string VariantName { get; }

        decimal MinimumTime { get; }

        // waste heat?

        /// <summary>
        /// Input Products
        /// </summary>
        IList<IProcessProduct> InputProducts { get; }

        /// <summary>
        /// Input Wants
        /// </summary>
        IList<IProcessWant> InputWants { get; }

        /// <summary>
        /// Capital Products.
        /// </summary>
        IList<IProcessProduct> CapitalProducts { get; }

        /// <summary>
        /// Capital Wants (IE, non-consumable products with the proper want attached).
        /// </summary>
        IList<IProcessWant> CapitalWants { get; }

        /// <summary>
        /// Product Outputs.
        /// </summary>
        IList<IProcessProduct> Outputs { get; }

        /// <summary>
        /// Process Tags
        /// </summary>
        IList<IAttachedProcessTag> Tags { get; }

        /// <summary>
        /// The Skill the process uses.
        /// </summary>
        ISkill Skill { get; }

        /// <summary>
        /// The minimum level of the skill.
        /// </summary>
        int SkillMinimum { get; }

        /// <summary>
        /// The maximum level of the skill
        /// </summary>
        int SkillMaximum { get; }

        /// <summary>
        /// Whether the process is allowed to produce fractional goods.
        /// </summary>
        bool Fractional { get; }

        // Division of Labor
        // split into it's own class for more flexible modification.
        // TODO Division of labor

        // Automation
        // Split into it's own class for easier modification.
        // TODO Automation

        // Technology connections
    }
}
