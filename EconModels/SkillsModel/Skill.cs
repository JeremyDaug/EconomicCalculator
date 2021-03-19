using EconModels.JobModels;
using EconModels.ProductModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.SkillsModel
{
    public class Skill
    {
        public Skill()
        {
            ValidLabors = new List<Product>();
            RelationChild = new List<Skill>();
            RelationParent = new List<Skill>();
        }

        /// <summary>
        /// The Id of the Skill
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The name of the Skill
        /// </summary>
        [Required, StringLength(maximumLength: 30, MinimumLength = 3)]
        [Index(IsUnique = true)]
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
        public virtual ICollection<Product> ValidLabors { get; set; }

        // Self referential Many to many.
        public virtual ICollection<Skill> RelationChild { get; set; }

        public virtual ICollection<Skill> RelationParent { get; set; }

        // Navigation Properties
        // To connect back to job.Skill
        public virtual ICollection<Job> SkillsJobs { get; set; }
    }
}
