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
    public class JobDTO : IJobDTO
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public JobDTO()
        {
            ProcessNames = new List<string>();
            ProcessIds = new List<int>();
        }

        /// <summary>
        /// The unique Id of the job
        /// </summary>
        [JsonIgnore]
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
        /// Retrieve the processes in a nicer format.
        /// </summary>
        [JsonIgnore]
        public string ProcessesString
        {
            get
            {
                if (ProcessNames != null)
                    return string.Join("\n", ProcessNames);
                return "";
            }
        }

        /// <summary>
        /// The Id's of the processes which are part of the job.
        /// </summary>
        [JsonIgnore]
        public List<int> ProcessIds { get; set; }

        public static (string Name, string VariantName) GetJobNames(string name)
        {
            if (name.Contains("("))
            {
                var names = name.Split('(');
                var primaryName = names[0];
                var varName = names[1].TrimEnd(')');
                return (primaryName, varName);
            }
            else
                return (name, "");
        }

        public string FullName()
        {
            var result = Name;
            if (!string.IsNullOrWhiteSpace(VariantName))
                result += "(" + VariantName + ")";
            return result;
        }
    }
}
