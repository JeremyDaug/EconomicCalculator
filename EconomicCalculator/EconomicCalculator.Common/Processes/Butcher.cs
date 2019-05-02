using System;
using System.Collections.Generic;

namespace EconomicCalculator.Common.Processes
{
    /// <summary>
    /// Butcher class manager
    /// </summary>
    public class Butcher
    {
        /// <summary>
        /// Name of what is being butchered.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The variant of the butchering.
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// What Animal is being butchered.
        /// </summary>
        /// <remarks>
        /// Animal must be made before it can be called.
        /// Animals are too complicated to keep around.
        /// </remarks>
        public string Animal { get; set; }

        /// <summary>
        /// What Products are made, how much, and their price.
        /// </summary>
        public List<Tuple<string, double, double>> Products { get; set; }

        /// <summary>
        /// The price multiplier of the products from the animal.
        /// </summary>
        public double PriceMultiplier { get; set; }
    }
}
