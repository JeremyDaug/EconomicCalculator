using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;

namespace EconomicSim.Objects.Jobs
{
    /// <summary>
    /// Read Only Job Interface.
    /// </summary>
    [JsonConverter(typeof(JobJsonConverter))]
    public interface IJob
    {
        /// <summary>
        /// Job Id.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Name of the Job.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Variant name for the job, must be unique when combined
        /// with primary name.
        /// </summary>
        string VariantName { get; }

        /// <summary>
        /// The primary labor the job uses.
        /// </summary>
        IProduct Labor { get; }

        /// <summary>
        /// The Skill the job primarily uses.
        /// </summary>
        ISkill Skill { get; }

        /// <summary>
        /// The Processes that are part of this job.
        /// </summary>
        IReadOnlyList<IProcess> Processes { get; }
    }
}
