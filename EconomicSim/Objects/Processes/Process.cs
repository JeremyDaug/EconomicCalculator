using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes.ProcessTags;
using EconomicSim.Objects.Skills;
using EconomicSim.Objects.Technology;

namespace EconomicSim.Objects.Processes
{
    /// <summary>
    /// Process Data Class
    /// </summary>
    [JsonConverter(typeof(ProcessJsonConverter))]
    internal class Process : IProcess
    {
        public Process()
        {
            ProcessTags = new List<ProcessTag>();
            ProcessProducts = new List<ProcessProduct>();
            ProcessWants = new List<ProcessWant>();
        }
        
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
        public List<ProcessProduct> ProcessProducts { get; set; }
        IReadOnlyList<IProcessProduct> IProcess.ProcessProducts => ProcessProducts;

        /// <summary>
        /// All the wants that go into or come out of the process.
        /// </summary>
        public List<ProcessWant> ProcessWants { get; set; }
        IReadOnlyList<IProcessWant> IProcess.ProcessWants => ProcessWants;

        /// <summary>
        /// Input Products, consumed by the process.
        /// </summary>
        public List<ProcessProduct> InputProducts
            => ProcessProducts.Where(x => x.Part == ProcessPartTag.Input)
                .ToList();
        IReadOnlyList<IProcessProduct> IProcess.InputProducts
            => InputProducts;

        /// <summary>
        /// Capital Products, used, but not consumed by the process.
        /// </summary>
        public List<ProcessProduct> CapitalProducts
            => ProcessProducts.Where(x => x.Part == ProcessPartTag.Capital)
                .ToList();

        IReadOnlyList<IProcessProduct> IProcess.CapitalProducts
            => CapitalProducts;

        /// <summary>
        /// Output Products, resulting products from the process.
        /// </summary>
        public List<ProcessProduct> OutputProducts
            => ProcessProducts.Where(x => x.Part == ProcessPartTag.Output)
                .ToList();

        IReadOnlyList<IProcessProduct> IProcess.OutputProducts
            => OutputProducts;

        /// <summary>
        /// Input Wants, consumed by the process.
        /// </summary>
        public List<ProcessWant> InputWants
            => ProcessWants.Where(x => x.Part == ProcessPartTag.Input)
                .ToList();

        IReadOnlyList<IProcessWant> IProcess.InputWants
            => InputWants;

        /// <summary>
        /// Capital Wants, used by the process.
        /// </summary>
        public List<ProcessWant> CapitalWants
            => ProcessWants.Where(x => x.Part == ProcessPartTag.Capital)
                .ToList();
        IReadOnlyList<IProcessWant> IProcess.CapitalWants => CapitalWants;

        /// <summary>
        /// Output Wants, created by the process.
        /// </summary>
        public List<ProcessWant> OutputWants
            => ProcessWants.Where(x => x.Part == ProcessPartTag.Output)
            .ToList();
        IReadOnlyList<IProcessWant> IProcess.OutputWants => OutputWants;

        #endregion InCapOut

        /// <summary>
        /// The Process's Tags.
        /// </summary>
        public List<ProcessTag> ProcessTags { get; set; }
        IReadOnlyList<ProcessTag> IProcess.ProcessTags => ProcessTags;

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
            if (string.IsNullOrWhiteSpace(VariantName))
                return Name;
            return Name + "(" + VariantName + ")";
        }
    }
}
