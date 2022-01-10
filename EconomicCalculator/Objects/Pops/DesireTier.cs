using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Pops
{
    /// <summary>
    /// The level that a desire is for a pop.
    /// </summary>
    public enum DesireTier
    {
        /// <summary>
        /// Desires needed by a pop to survive.
        /// </summary>
        Life,
        /// <summary>
        /// Desires needed to make life bearable.
        /// </summary>
        Daily,
        /// <summary>
        /// Desires that make life worth living.
        /// </summary>
        Luxury
    }
}
