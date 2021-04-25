using EconModels.Enums;
using EconModels.SkillsModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebInterface.Models
{
    public class JobModel
    {
        public int Id { get; set; }

        [DisplayName("Job Name")]
        [Required, StringLength(maximumLength: 30, MinimumLength = 3)]
        public string Name { get; set; }

        [DisplayName("Job Type")]
        public JobTypes JobType { get; set; }

        [DisplayName("Job Category")]
        public JobCategory JobCategory { get; set; }

        // processes
        public int[] SelectedProcessIds { get; set; }
        public IEnumerable<SelectListItem> ProcessList { get; set; }

        public int SkillId { get; set; }

        [DisplayName("Skill")]
        public Skill Skill { get; set; }

        [Required, Range(0, float.MaxValue)]
        [DisplayName("Skill Level")]
        public int SkillLevel { get; set; }

        // Labor Products
        public int[] SelectedLaborIds { get; set; }
        public IEnumerable<SelectListItem> LaborList { get; set; }

        // Job List
        public int[] SelectedRelatedJobIds { get; set; }
        public IEnumerable<SelectListItem> Jobs { get; set; }
    }
}