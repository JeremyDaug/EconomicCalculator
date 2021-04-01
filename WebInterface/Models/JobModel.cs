using EconModels.Enums;
using EconModels.SkillsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebInterface.Models
{
    public class JobModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public JobTypes JobType { get; set; }

        public JobCategory JobCategory { get; set; }

        // processes
        public int[] SelectedProcessIds { get; set; }
        public IEnumerable<SelectListItem> ProcessList { get; set; }

        public int SkillId { get; set; }

        public Skill Skill { get; set; }

        public int SkillLevel { get; set; }

        // Labor Products
        public int[] SelectedLaborIds { get; set; }
        public IEnumerable<SelectListItem> LaborList { get; set; }

        // Job List
        public int[] SelectedRelatedJobIds { get; set; }
        public IEnumerable<SelectListItem> Jobs { get; set; }
    }
}