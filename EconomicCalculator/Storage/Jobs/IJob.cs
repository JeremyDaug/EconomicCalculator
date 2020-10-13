using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;
using EconomicCalculator.Storage;
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
        /// The Daily Input Requirements of the Job.
        /// </summary>
        IProductAmountCollection Inputs { get; }

        /// <summary>
        /// The Outputs produced if All inputs met.
        /// </summary>
        IProductAmountCollection Outputs { get; }

        /// <summary>
        /// Any Capital Needed to start the job. 
        /// Products are not consumed every day, instead having a chance of breaking.
        /// </summary>
        IProductAmountCollection Capital { get; }

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
    }
}
