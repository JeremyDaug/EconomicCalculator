using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Objects.Products;

namespace EconomicCalculator.Objects.Pops.PopNeeds
{
    /// <summary>
    /// Pop product need Data Object
    /// </summary>
    internal class PopNeed : IPopNeed
    {
        /// <summary>
        /// The product Desired.
        /// </summary>
        public IProduct Product { get; set; }

        /// <summary>
        /// The amount desired.
        /// </summary>
        public decimal Amount { get; set; }
    }
}
