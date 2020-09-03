using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;
using EconomicCalculator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage
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
        /// The storage of what capital goods have been satisfied.
        /// </summary>
        IProductAmountCollection CapitalStorage { get; }

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

        // Placeholder for skill requirements.

        /// <summary>
        /// The Total required labor of the job ignoring skill level.
        /// </summary>
        double TotalLaborRequired();
    }
}
