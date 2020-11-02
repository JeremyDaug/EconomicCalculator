using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Enums;
using EconomicCalculator.Storage.Products;

namespace EconomicCalculator.Storage.Jobs
{
    internal class Job : IJob
    {
        #region GeneralData

        /// <summary>
        /// The Id of the job.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The name of the job.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of job that this is, which defines how
        /// inputs, outputs, capital, and labor are used.
        /// </summary>
        public JobTypes JobType { get; }

        /// <summary>
        /// The Category of the job.
        /// </summary>
        public JobCategory JobCategory { get; }

        /// <summary>
        /// The inputs required for the job's process.
        /// </summary>
        public IReadOnlyProductAmountCollection Inputs { get; }

        /// <summary>
        /// The capital required for the job's process.
        /// </summary>
        public IReadOnlyProductAmountCollection Capital { get; }

        /// <summary>
        /// The goods output by the job's process.
        /// </summary>
        public IReadOnlyProductAmountCollection Outputs { get; }

        /// <summary>
        /// Processes the job does.
        /// </summary>
        public IProcess Process { get; }

        /// <summary>
        /// The name of the skill for the job.
        /// </summary>
        public string SkillName { get; }

        /// <summary>
        /// The Skill required to work the job.
        /// </summary>
        public int SkillLevel { get; }

        /// <summary>
        /// How much work per unit of the job is needed.
        /// </summary>
        public double LaborRequirements { get; }

        /// <summary>
        /// The jobs related to this job. People working this job can switch to one of these jobs trivially.
        /// </summary>
        public IReadOnlyList<IJob> RelatedJobs { get; } // TODO May remove this in favor of just searching jobs with the same skill name.

        #endregion GeneralData

        public bool Equals(IJob other)
        {
            return this.Id == other.Id;
        }

        public bool Equals(IJob x, IJob y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(IJob obj)
        {
            return obj.Id.GetHashCode();
        }

        public void LoadFromSql(SqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
