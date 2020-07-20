using System.Collections.Generic;

namespace EconomicCalculator.Intermediaries
{
    /// <summary>
    /// Products are goods which can be bought, sold, and consumed.
    /// </summary>
    public interface IProduct
    {
        /// <summary>
        /// The name of the product.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A unique name to differentiate between the same product made through
        /// different means, in different locations, or by methods.
        /// </summary> 
        // string VariantName { get; }
        
        /// <summary>
        /// What is the name or type of unit used to buy and
        /// sell this product.
        /// </summary>
        string UnitName { get; }

        /// <summary>
        /// The current market price of the product.
        /// </summary>
        double CurrentPrice { get; }

        // Components Implement Later
        /// <summary>
        /// The components that make up the Product and can be replaced individiually. May be empty or null.
        /// </summary>
        // IList<IProduct> Components { get; }

        /// <summary>
        /// The number of each component.
        /// </summary>
        // IDictionary<string, double> ComponentCounts { get; }

        // Failure metrics
        /// <summary>
        /// The Average time for the item to break irrepairably.
        /// </summary>
        double MTTF { get; }

        /// <summary>
        /// The chance that on any given day, the product will break. Calculated from <see cref="MTTF"/>.
        /// </summary>
        double DailyFailureChance { get; }

        /// <summary>
        /// The probability that after a number of days, failure will occur.
        /// </summary>
        /// <param name="days">How many days to calculate for.</param>
        /// <returns>The probability that the product breaks.</returns>
        double FailureProbability(double days);
    }
}