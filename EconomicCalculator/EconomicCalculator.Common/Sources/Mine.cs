using EconomicCalculator.Common.Resource;
using System;
using System.Collections.Generic;

namespace EconomicCalculator.Common.Sources
{
    /// <summary>
    /// Mine Class
    /// </summary>
    public class Mine
    {
        /// <summary>
        /// What kind of mine it is.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Variant of the Mine (what specific mine it is)
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// What job works here.
        /// </summary>
        public string Job { get; set; }

        /// <summary>
        /// What the mine produces, how much per day, and how much it's unit price in UC.
        /// </summary>
        public List<InputOutputs> Products { get; set; }
    }
}
