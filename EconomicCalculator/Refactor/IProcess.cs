using EconomicCalculator.Intermediaries;
using EconomicCalculator.Storage;
using EconomicCalculator.Storage.Jobs;
using System.Collections.Generic;

namespace EconomicCalculator.Generators
{
    /// <summary>
    /// Processes that turn product(s) into other product(s) via labor.
    /// </summary>
    public interface IProcess : IJob
    {
        /// <summary>
        /// The name of the process
        /// </summary>
        new string Name { get; }

        /// <summary>
        /// The variant process.
        /// </summary> TODO, add later
        // string VariantName { get; }

        // Product inputs
        /// <summary>
        /// The product inputs consumed to vmake the products.
        /// </summary>
        new IList<IProduct> Inputs { get; }

        /// <summary>
        /// The Amount of each input product required in their units.
        /// </summary>
        new IDictionary<string, double> InputRequirements { get; }

        // Product Outputs
        /// <summary>
        /// What the Products the process produces.
        /// </summary>
        new IList<IProduct> Outputs { get; }

        /// <summary>
        /// How much of each output product is produced (in their units).
        /// </summary>
        new IDictionary<string, double> OutputResults { get; }

        // Labor Requirements
        /// <summary>
        /// The labor requirements of the process.
        /// </summary>
        new double LaborRequirements { get; }

        // Equipment Requirements To Add in Later
        /// <summary>
        /// Products required to do the process, not consumed but breakable.
        /// </summary>
        // IList<IProduct> Capital { get; }

        /// <summary>
        /// The required Capital for the process
        /// </summary>
        // IDictionary<string, double> CapitalRequirements { get; }

        /// <summary>
        /// The cost of buying the inputs.
        /// </summary>
        /// <returns>The price of all inputs for the process.</returns>
        double ProductionCost();
    }
}