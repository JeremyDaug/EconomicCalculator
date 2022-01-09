using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Objects.Wants;

namespace EconomicCalculator.Objects.Pops.PopWants
{
    /// <summary>
    /// Pop want Data Object
    /// </summary>
    internal class PopWant : IPopWants
    {
        /// <summary>
        /// The want desired.
        /// </summary>
        public IWant Want { get; set; }

        /// <summary>
        /// The amount desired.
        /// </summary>
        public decimal Amount { get; set; }
    }
}
