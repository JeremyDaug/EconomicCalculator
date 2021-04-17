using EconModels.JobModels;
using EconModels.ProductModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            SkillsJobs = new List<Job>();
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
        [DisplayName("Valid Labors")]
        public virtual ICollection<Product> ValidLabors { get; set; }

        // Self referential Many to many.
        /// <summary>
        /// Skills which are related to this skill and have lower
        /// friction when shifting between.
        /// </summary>
        [DisplayName("Related Skills")]
        public virtual ICollection<Skill> RelationChild { get; set; }

        // The other half of the self Referential Many to Many, combine with Relation Child for total list.
        public virtual ICollection<Skill> RelationParent { get; set; }

        // Navigation Properties
        // To connect back to job.Skill
        [DisplayName("Related Jobs")]  
        public virtual ICollection<Job> SkillsJobs { get; set; }

        /// <summary>
        /// Helper function for adding skill relations.
        /// Adds to both this skill and to <paramref name="relation"/>.
        /// </summary>
        /// <param name="relation">The skill this skill is related to.</param>
        public void AddSkillRelation(Skill relation)
        {
            // if already in list, don't add it again.
            if (RelationChild.Contains(relation))
                return;
            // add to this skill
            RelationChild.Add(relation);
            RelationParent.Add(relation);
            // add to the other skill, if it already exists in relation it
            // will not be added again.
            relation.AddSkillRelation(this);
        }

        public void RemoveSkillRelation(Skill skill)
        {
            if (!RelationChild.Contains(skill))
                return;

            RelationChild.Remove(skill);
            RelationParent.Remove(skill);

            skill.RemoveSkillRelation(skill);
        }

        public void ClearSkillRelations()
        {
            // remove myself from related skills
            foreach (var skill in RelationChild)
            {
                RemoveSkillRelation(skill);
            }
        }
    }
}
