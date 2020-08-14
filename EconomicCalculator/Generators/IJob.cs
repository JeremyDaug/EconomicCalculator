using EconomicCalculator.Enums;
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

        /// <summary>
        /// The type of job that this is (defines how it should calculate 
        /// </summary>
        JobTypes JobType { get; }

        /// <summary>
        /// Do work and make goods in the production.
        /// </summary>
        /// <param name="availableGoods">The goods available to the working population.</param>
        /// <param name="Pops">The number of people in the population.</param>
        /// <returns>The resulting change in goods for the population.</returns>
        IDictionary<string, double> Work(IDictionary<string, double> availableGoods, int Pops);
    }
}
