using EconModels.Enums;
using EconModels.ProcessModel;
using EconModels.ProductModel;
using EconModels.SkillsModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.JobModels
{
    public class Job
    {
        public Job()
        {
            Processes = new List<Process>();
            Labor = new List<Product>();
            RelatedParent = new List<Job>();
            RelatedChild = new List<Job>();
        }

        public int Id { get; set; }

        /// <summary>
        /// The name of the job (if any).
        /// </summary>
        [Required, StringLength(30, MinimumLength = 3)]
        [DisplayName("Job Name")]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        /// <summary>
        /// What kind of job this is which defines the logic behind
        /// the job. Does it create things, does it work on timetables
        /// does it extract from the ground or process goods, etc.
        /// </summary>
        [DisplayName("Job Type")]
        [Required]
        public JobTypes JobType { get; set; }

        /// <summary>
        /// The overarching category of the job, also helps define
        /// what the job does, but is more for general information.
        /// </summary>
        [DisplayName("Job Category")]
        [Required]
        public JobCategory JobCategory { get; set; }

        /// <summary>
        /// What processes does the job do.
        /// </summary>
        [DisplayName("Processes")]
        public virtual ICollection<Process> Processes { get; set; }

        /// <summary>
        /// The primary skill the job has.
        /// </summary>
        [Required]
        public int SkillId { get; set; }

        /// <summary>
        /// The Primary Skill the job has.
        /// </summary>
        [ForeignKey("SkillId")]
        [DisplayName("Skill")]
        public virtual Skill Skill { get; set; }

        /// <summary>
        /// What level of skill the job has.
        /// This is normally limited to 0-5, but certain skills
        /// may lower the minimum and maximum range.
        /// </summary>
        [Required, Range(0, float.MaxValue)]
        [DisplayName("Skill Level")]
        public int SkillLevel { get; set; }

        /// <summary>
        /// What labor or service the job provides to the market.
        /// This labor product must already exist in the database.
        /// </summary>
        [DisplayName("Labors")]
        public virtual ICollection<Product> Labor { get; set; }
        
        // Many to many self refernce for related jobs.
        [DisplayName("Related Jobs")]
        public virtual ICollection<Job> RelatedChild { get; set; }
        public virtual ICollection<Job> RelatedParent { get; set; }

        // This class is an abstract job and so specific breakdowns
        // are not needed here, but rather in the population group
        // the job is connected to. 

        public void AddRelatedJob(Job otherJob)
        {
            if (RelatedChild.Contains(otherJob))
                return;

            RelatedChild.Add(otherJob);
            RelatedParent.Add(otherJob);

            otherJob.AddRelatedJob(this);
        }

        public void RemoveJobRelation(Job job)
        {
            if (!RelatedChild.Contains(job))
                return;

            RelatedChild.Remove(job);
            RelatedParent.Remove(job);

            job.RemoveJobRelation(this);
        }

        public void ClearJobRelations()
        {
            foreach (var job in RelatedChild)
            {
                RemoveJobRelation(job);
            }
        }

        public void ClearProcessRelations()
        {
            // go through each process and remove this job.
            foreach (var proc in Processes)
            {
                proc.Jobs.Remove(this);
            }

            // then clear jobs.
            Processes.Clear();
        }

        public void ClearLaborRelations()
        {
            // remove this job from all related labors
            foreach (var labor in Labor)
            {
                labor.Jobs.Remove(this);
            }

            // then clear labors
            Labor.Clear();
        }

        public void AddProcess(Process process)
        {
            Processes.Add(process);
            process.Jobs.Add(this);
        }

        public void AddLabor(Product labor)
        {
            Labor.Add(labor);
            labor.Jobs.Add(this);
        }
    }
}
