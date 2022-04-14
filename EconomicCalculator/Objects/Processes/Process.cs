using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Objects.Processes.ProcessTags;
using EconomicCalculator.Objects.Skills;
using EconomicCalculator.Objects.Technology;

namespace EconomicCalculator.Objects.Processes
{
    /// <summary>
    /// Process Data Class
    /// </summary>
    internal class Process : IProcess
    {
        /// <summary>
        /// The Process Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Process Name, must be a unique combo with Variant Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Process Variant Name, must be unique combo with Name.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// The Minimum time the Process Takes.
        /// </summary>
        public decimal MinimumTime { get; set; }

        #region InCapOut

        /// <summary>
        /// All the products that go into or come out of the process.
        /// </summary>
        public IReadOnlyList<IProcessProduct> ProcessProducts { get; }

        /// <summary>
        /// All the wants that go into or come out of the process.
        /// </summary>
        public IReadOnlyList<IProcessWant> ProcessWants { get; }

        /// <summary>
        /// Input Products, consumed by the process.
        /// </summary>
        public IReadOnlyList<IProcessProduct> InputProducts
            => ProcessProducts.Where(x => x.Part == ProcessPartTag.Input)
            .ToList();

        /// <summary>
        /// Capital Products, used, but not consumed by the process.
        /// </summary>
        public IReadOnlyList<IProcessProduct> CapitalProducts
            => ProcessProducts.Where(x => x.Part == ProcessPartTag.Capital)
            .ToList();

        /// <summary>
        /// Output Products, resulting products from the process.
        /// </summary>
        public IReadOnlyList<IProcessProduct> OutputProducts
            => ProcessProducts.Where(x => x.Part == ProcessPartTag.Output)
            .ToList();

        /// <summary>
        /// Input Wants, consumed by the process.
        /// </summary>
        public IReadOnlyList<IProcessWant> InputWants
            => ProcessWants.Where(x => x.Part == ProcessPartTag.Input)
            .ToList();

        /// <summary>
        /// Capital Wants, used by the process.
        /// </summary>
        public IReadOnlyList<IProcessWant> CapitalWants
            => ProcessWants.Where(x => x.Part == ProcessPartTag.Capital)
            .ToList();

        /// <summary>
        /// Output Wants, created by the process.
        /// </summary>
        public IReadOnlyList<IProcessWant> OutputWants
            => ProcessWants.Where(x => x.Part == ProcessPartTag.Output)
            .ToList();

        #endregion InCapOut

        /// <summary>
        /// The Process's Tags.
        /// </summary>
        public IReadOnlyList<ProcessTag> ProcessTags { get; set; }

        /// <summary>
        /// The Skill the proces Uses.
        /// </summary>
        public ISkill Skill { get; set; }

        /// <summary>
        /// The minimum level of the skill.
        /// </summary>
        public decimal SkillMinimum { get; set; }

        /// <summary>
        /// The Maximum level of a skill for the process.
        /// </summary>
        public decimal SkillMaximum { get; set; }

        /// <summary>
        /// Whether the process is functional or not.
        /// </summary>
        public bool Fractional
        {
            get
            {
                return
                    !InputProducts.Any(x => !x.Product.Fractional) ||
                    !OutputProducts.Any(x => !x.Product.Fractional);
            }
        }

        /// <summary>
        /// A quick description of the process.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Location of the Icon representing the process.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The tech required see/do this process.
        /// </summary>
        public ITechnology TechRequirement { get; set; }
        ITechnology IProcess.TechRequirement => TechRequirement;

        /// <summary>
        /// Retrieve the name of the process.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return Name + "(" + VariantName + ")";
        }
    }
}
