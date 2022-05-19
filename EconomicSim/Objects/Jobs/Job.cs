using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;

namespace EconomicSim.Objects.Jobs
{
    /// <summary>
    /// Job Data Class
    /// </summary>
    [JsonConverter(typeof(JobJsonConverter))]
    public class Job : IJob
    {
        public Job()
        {
            Processes = new List<Process>();
        }

        /// <summary>
        /// Job Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the job.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Variant Name of a job, should be unique with Primary name.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// The primary Labor Product of the job.
        /// </summary>
        public IProduct Labor { get; set; }

        /// <summary>
        /// The skill the job uses.
        /// </summary>
        public ISkill Skill { get; set; }

        /// <summary>
        /// The PRocesses that are done by the job.
        /// </summary>
        public List<Process> Processes { get; set; }
        IReadOnlyList<IProcess> IJob.Processes => Processes;

        public string GetName()
        {
            if (!string.IsNullOrWhiteSpace(VariantName))
                return $"{Name}({VariantName})";
            return Name;
        }
    }
}
