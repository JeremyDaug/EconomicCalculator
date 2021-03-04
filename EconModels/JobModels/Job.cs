using EconModels.Enums;
using EconModels.ProcessModel;
using EconModels.ProductModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.JobModels
{
    public class Job
    {
        public Job()
        {
            RelatedJobs = new List<Job>();
        }

        public int Id { get; set; }

        [Required, StringLength(30, MinimumLength = 3)]
        [DisplayName("Job Name")]
        public string Name { get; set; }

        [DisplayName("Job Type")]
        public JobTypes JobType { get; set; }

        [DisplayName("Job Category")]
        public JobCategory JobCategory { get; set; }

        public int ProcessId { get; set; }

        [DisplayName("Process")]
        public virtual Process Process { get; set; }

        [Required, StringLength(30, MinimumLength = 3)]
        public string SkillName { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int SkillLevel { get; set; }

        [Required]
        public int ServiceId { get; set; }

        /// <summary>
        /// What labor or service the job provides to the market.
        /// This labor product must already exist in the database.
        /// </summary>
        [DisplayName("Service")]
        public virtual Product Service { get; set; }

        public virtual ICollection<Job> RelatedJobs { get; set; }
    }
}
