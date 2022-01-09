using EconomicCalculator.Objects.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Pops.PopNeeds
{
    /// <summary>
    /// Read only Population Need Interface.
    /// </summary>
    public interface IPopNeed
    {
        /// <summary>
        /// The product needed.
        /// </summary>
        IProduct Product { get; }

        /// <summary>
        /// how much of the product is needed.
        /// </summary>
        decimal Amount { get; }
    }
}
