using EconomicCalculator.Common.Resource;
using System;
using System.Collections.Generic;

namespace EconomicCalculator.Common.Processes
{
    /// <summary>
    /// Craft class, for taking input products and making output produts.
    /// </summary>
    public class Craft
    {
        /// <summary>
        /// Name of the craft (and/or what is being made)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Variant of the Craft.
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// What job does this craft, their wage, how many are needed, and how long they need to work
        /// </summary>
        public Tuple<string, double, double, double> Job { get; set; }

        /// <summary>
        /// The inputs, the amount of them, and their price.
        /// </summary>
        public IList<Tuple<string, double, double>> Inputs { get; set; }

        /// <summary>
        /// the outputs, the amount of them, and their price.
        /// </summary>
        public IList<Tuple<string, double, double>> Outputs { get; set; }

        /// <summary>
        /// How long it takes to craft in days.
        /// </summary>
        public double TimeInDays { get; set; }

        /// <summary>
        /// How much the price multiplies by crafting.
        /// </summary>
        public double PriceMultiplier { get; set; }
    }
}
