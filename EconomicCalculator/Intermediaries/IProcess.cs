using System.Collections.Generic;

namespace EconomicCalculator.Intermediaries
{
    /// <summary>
    /// Processes that turn product(s) into other product(s) via labor.
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// The name of the process
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The variant process.
        /// </summary> TODO, add later
        // string VariantName { get; }

        // Product inputs
        /// <summary>
        /// The product inputs consumed to vmake the products.
        /// </summary>
        IList<IProduct> Inputs { get; }

        /// <summary>
        /// The Amount of each input product required in their units.
        /// </summary>
        IDictionary<string, double> InputRequirements { get; }

        // Product Outputs
        /// <summary>
        /// What the Products the process produces.
        /// </summary>
        IList<IProduct> Outputs { get; }

        /// <summary>
        /// How much of each output product is produced (in their units).
        /// </summary>
        IDictionary<string, double> OutputResults { get; }

        // Labor Requirements
        /// <summary>
        /// The labor requirements of the process.
        /// </summary>
        double LaborRequirements { get; }

        // Equipment Requirements To Add in Later
        /// <summary>
        /// Products required to do the process, not consumed but breakable.
        /// </summary>
        // IList<IProduct> Capital { get; }

        /// <summary>
        /// The required Capital for the process
        /// </summary>
        // IDictionary<string, double> CapitalRequirements { get; }
    }
}