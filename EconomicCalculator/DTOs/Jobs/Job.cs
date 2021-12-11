using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Jobs
{
    /// <summary>
    /// Job DTO class
    /// </summary>
    public class Job : IJob
    {
        /// <summary>
        /// The unique Id of the job
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The primary name of the job.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The secondary name for the job, must be unique when combined with primary.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// The primary labor they do.
        /// </summary>
        [JsonIgnore]
        public int LaborId { get; set; }

        /// <summary>
        /// The name of the labor they do.
        /// </summary>
        public string Labor { get; set; }

        /// <summary>
        /// The Id of their primary skill and what a pop in this job is
        /// considered to have experience in.
        /// </summary>
        [JsonIgnore]
        public int SkillId { get; set; }

        /// <summary>
        /// The Name of their primary skill and what a pop in this job is
        /// considered to have experience in.
        /// </summary>
        public string Skill { get; set; }

        /// <summary>
        /// The processes which are part of the jod.
        /// </summary>
        public List<string> ProcessNames { get; set; }

        /// <summary>
        /// The Id's of the processes which are part of the job.
        /// </summary>
        [JsonIgnore]
        public List<int> ProcessIds { get; set; }
    }
}
