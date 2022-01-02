using EconomicCalculator.Objects.Processes.ProcessTags;
using EconomicCalculator.Objects.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Processes
{
    /// <summary>
    /// Readonly Process Interface
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// The Id of the Process
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the process, must form a unique combination 
        /// with <seealso cref="VariantName"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The variant name of this process, if it's similar, but not
        /// exactly the same.
        /// </summary>
        /// <remarks>
        /// Must form unique combination with <seealso cref="Name"/>.
        /// </remarks>
        string VariantName { get; }

        /// <summary>
        /// The minimum amount of time the process takes up.
        /// 1 equals 1 day.
        /// </summary>
        decimal MinimumTime { get; }

        #region InCapOut

        /// <summary>
        /// All the products that go into or come out of the process.
        /// </summary>
        IReadOnlyList<IProcessProduct> ProcessProducts { get; }

        /// <summary>
        /// All the wants that go into or come out of the process.
        /// </summary>
        IReadOnlyList<IProcessWant> ProcessWants { get; }

        /// <summary>
        /// Input Products, consumed by the process.
        /// </summary>
        IReadOnlyList<IProcessProduct> InputProducts { get; }

        /// <summary>
        /// Capital Products, used, but not consumed by the process.
        /// </summary>
        IReadOnlyList<IProcessProduct> CapitalProducts { get; }

        /// <summary>
        /// Output Products, resulting products from the process.
        /// </summary>
        IReadOnlyList<IProcessProduct> OutputProducts { get; }

        /// <summary>
        /// Input Wants, consumed by the process.
        /// </summary>
        IReadOnlyList<IProcessWant> InputWants { get; }

        /// <summary>
        /// Capital Wants, used by the process.
        /// </summary>
        IReadOnlyList<IProcessWant> CapitalWants { get; }

        /// <summary>
        /// Output Wants, created by the process.
        /// </summary>
        IReadOnlyList<IProcessWant> OutputWants { get; }

        #endregion InCapOut

        /// <summary>
        /// The process Tags of the Process
        /// </summary>
        IReadOnlyList<ProcessTag> ProcessTags { get; }

        /// <summary>
        /// The Skill the Process Uses.
        /// </summary>
        ISkill Skill { get; }

        /// <summary>
        /// The minimum level of the skill.
        /// </summary>
        decimal SkillMinimum { get; }

        /// <summary>
        /// The maximum level of the skill
        /// </summary>
        decimal SkillMaximum { get; }

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

        /// <summary>
        /// Description of the process
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The Icon for the process
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// Get the Name(VariantName) of the process in
        /// standard form.
        /// </summary>
        /// <returns></returns>
        string GetName();
    }
}
