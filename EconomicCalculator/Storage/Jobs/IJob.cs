using EconomicCalculator.Enums;
using EconomicCalculator.Storage;
using EconomicCalculator.Storage.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Jobs
{
    /// <summary>
    /// An interface for anything that can be worked as a job.
    /// </summary>
    public interface IJob : IEquatable<IJob>, ISqlReader, IEqualityComparer<IJob>
    {
        #region GeneralData

        /// <summary>
        /// The Id of the job.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The name of the job.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The type of job that this is, which defines how
        /// inputs, outputs, capital, and labor are used.
        /// </summary>
        JobTypes JobType { get; }

        /// <summary>
        /// The inputs required for the job's process.
        /// </summary>
        IProductAmountCollection Inputs { get; }

        /// <summary>
        /// The capital required for the job's process.
        /// </summary>
        IProductAmountCollection Capital { get; }

        /// <summary>
        /// The goods output by the job's process.
        /// </summary>
        IProductAmountCollection Outputs { get; }

        /// <summary>
        /// Processes the job does.
        /// </summary>
        IProcess Process { get; }

        /// <summary>
        /// The name of the skill for the job.
        /// </summary>
        string SkillName { get; }

        /// <summary>
        /// The Skill required to work the job.
        /// </summary>
        int SkillLevel { get; }

        /// <summary>
        /// How much work per unit of the job is needed.
        /// </summary>
        double LaborRequirements { get; }

        /// <summary>
        /// The jobs related to this job. People working this job can switch to one of these jobs trivially.
        /// </summary>
        IReadOnlyList<IJob> RelatedJobs { get; } // TODO May remove this in favor of just searching jobs with the same skill name.

        #endregion GeneralData

        #region Aggregation

        /// <summary>
        /// The Daily input needs of the job. <see cref="Inputs"/> / <see cref="LaborRequirements"/>.
        /// </summary>
        /// <returns>The DailyInput Needs of the job.</returns>
        IProductAmountCollection DailyInputNeeds();

        /// <summary>
        /// The Daily Input needs of the job for a number of peopel.
        /// </summary>
        /// <param name="amount">The number of pops.</param>
        /// <returns>THe Daily Input needs for a population.</returns>
        IProductAmountCollection DailyInputNeedsForPops(double amount);

        /// <summary>
        /// The amount of capital needed for a population.
        /// </summary>
        /// <param name="amount">The size of a population.</param>
        /// <returns>The total capital needs of said pop.</returns>
        IProductAmountCollection CapitalNeedsForPops(double amount);

        #endregion Aggregation
    }
}
