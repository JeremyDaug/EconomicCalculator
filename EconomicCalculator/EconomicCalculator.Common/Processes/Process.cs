﻿using EconomicCalculator.Common.Resource;
using System;
using System.Collections.Generic;

namespace EconomicCalculator.Common.Processes
{
    /// <summary>
    /// A process, usually simplified reduction of crops from product(s) to product(s)
    /// Counted 
    /// </summary>
    public class Process
    {
        /// <summary>
        /// Name of the Process
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The specific variant of the process.
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// Input Products 
        /// Name, Price
        /// </summary>
        public IList<Tuple<string, double>> InputsProducts { get; set; }

        /// <summary>
        /// Output Products
        /// name, price
        /// </summary>
        public IList<Tuple<string, double>> OutputProducts { get; set; }

        /// <summary>
        /// How much 1 unit is reduced in weight.
        /// </summary>
        public double Reduction { get; set; }

        /// <summary>
        /// How much can be done in a day.
        /// </summary>
        public double ProductionPerDay { get; set; }

        /// <summary>
        /// How much of the Product made goes to the Processor.
        /// </summary>
        public double ProcessorsCut { get; set; }

        /// <summary>
        /// How much the result is multiplied in value.
        /// </summary>
        public double PriceMultiplier { get; set; }

        /// <summary>
        /// What job does this.
        /// </summary>
        public Tuple<string, double> Job { get; set; }
    }
}
