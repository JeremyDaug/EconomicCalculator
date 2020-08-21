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
    public interface IJob
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
        /// The Products required for the job.
        /// </summary>
        IList<IProduct> Inputs { get; }

        /// <summary>
        /// The Amount of each input required for the job.
        /// </summary>
        IDictionary<string, double> InputRequirements { get; }

        /// <summary>
        /// The Outputs of the job.
        /// </summary>
        IList<IProduct> Outputs { get; }

        /// <summary>
        /// The Amount of each Output product from the job.
        /// </summary>
        IDictionary<string, double> OutputResults { get; }

        /// <summary>
        /// How much work per unit of the job is needed, and what skill level it must be.
        /// </summary>
        IList<double> LaborRequirements { get; }

        /// <summary>
        /// The type of job that this is (defines how labor and outputs are used)
        /// </summary>
        JobTypes JobType { get; }

        /// <summary>
        /// The Total required labor of the job ignoring skill level.
        /// </summary>
        double TotalLaborRequired();
    }
}
