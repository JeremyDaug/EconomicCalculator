using EconModels.Enums;
using EconModels.ProcessModel;
using System;
using System.Collections.Generic;
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
        public string Name { get; set; }

        public JobTypes JobType { get; set; }

        public JobCategory JobCategory { get; set; }

        [Required]
        public virtual Process Process { get; set; }

        [Required, StringLength(30, MinimumLength = 3)]
        public string SkillName { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int SkillLevel { get; set; }

        [Required, Range(0, double.MaxValue)]
        public double LaborRequirements { get; set; }

        public virtual ICollection<Job> RelatedJobs { get; set; }
    }
}
