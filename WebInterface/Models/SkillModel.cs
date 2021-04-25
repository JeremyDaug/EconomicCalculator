using EconModels.JobModels;
using EconModels.ProductModel;
using EconModels.SkillsModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebInterface.Models
{
    /// <summary>
    /// A copy of skill 
    /// </summary>
    public class SkillModel
    {
        /// <summary>
        /// The Id of the Skill
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the Skill
        /// </summary>
        [Required, StringLength(maximumLength: 30, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// A description of the skill.
        /// </summary>
        [Required, StringLength(maximumLength: 100)]
        public string Desc { get; set; }

        /// <summary>
        /// The minimum level of the skill.
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public int Min { get; set; }

        /// <summary>
        /// The Maximum level of the skill.
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public int Max { get; set; }

        /// <summary>
        /// What kinds of labors or services this skill can produce. This
        /// should only connect to products which are of type service or labor.
        /// </summary>
        public int[] SelectedLaborIds { get; set; }
        public IEnumerable<SelectListItem> LaborList { get; set; }

        // Self referential Many to many.
        /// <summary>
        /// Skills which are related to this skill and have lower
        /// friction when shifting between.
        /// </summary>
        public int[] SelectedSkillIds { get; set; }
        public IEnumerable<SelectListItem> RelatedSkills { get; set; }
    }
}