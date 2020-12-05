using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.JobModels
{
    public class Job
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public JobTypes JobType { get; set; }

        public JobCategory JobCategory { get; set; }

        public string SkillName { get; set; }

        public int SkillLevel { get; set; }

        public double LaborRequirements { get; set; }
    }
}
