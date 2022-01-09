using EconomicCalculator.Objects.Wants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Pops.PopWants
{
    /// <summary>
    /// Read Only Interface for Population Want.
    /// </summary>
    public interface IPopWants
    {
        /// <summary>
        /// The Want Desired.
        /// </summary>
        IWant Want { get; }

        /// <summary>
        /// The amount of the want desired.
        /// </summary>
        decimal Amount { get; }
    }
}
