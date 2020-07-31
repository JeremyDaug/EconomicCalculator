using EconomicCalculator.Intermediaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
{
    /// <summary>
    /// An interface for anything that can be worked as a job.
    /// </summary>
    public interface IJob
    {
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
        /// How much work per unit of the job is needed.
        /// </summary>
        double LaborRequirements { get; }
    }
}
