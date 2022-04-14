using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Jobs
{
    /// <summary>
    /// Read Only interface for jobs.
    /// </summary>
    public interface IJobDTO
    {
        /// <summary>
        /// The unique Id of the job
        /// </summary>
        [JsonIgnore]
        int Id { get; }

        /// <summary>
        /// The primary name of the job.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The secondary name for the job, must be unique when combined with primary.
        /// </summary>
        string VariantName { get; }

        /// <summary>
        /// The primary labor they do.
        /// </summary>
        [JsonIgnore]
        int LaborId { get; }

        /// <summary>
        /// The name of the labor they do.
        /// </summary>
        string Labor { get; }

        /// <summary>
        /// The Id of their primary skill and what a pop in this job is
        /// considered to have experience in.
        /// </summary>
        [JsonIgnore]
        int SkillId { get; }

        /// <summary>
        /// The Name of their primary skill and what a pop in this job is
        /// considered to have experience in.
        /// </summary>
        string Skill { get; }

        /// <summary>
        /// The processes which are part of the jod.
        /// </summary>
        List<string> ProcessNames { get; }

        /// <summary>
        /// Retrieve the processes in a nicer format.
        /// </summary>
        [JsonIgnore]
        string ProcessesString { get; }

        /// <summary>
        /// The Id's of the processes which are part of the job.
        /// </summary>
        [JsonIgnore]
        List<int> ProcessIds { get; }

        string FullName();
    }
}
