using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicSim.Enums
{
    /// <summary>
    /// Various Tags for the inputs, capital, and outputs of
    /// a process.
    /// </summary>
    public enum ProcessGoodTag
    {
        /// <summary>
        /// Default Tag, the product is required and the 
        /// ratio of it's satisfaction reduces the throughput
        /// of the entire process. It is consumed by the 
        /// process and does not produce it's failure 
        /// products.
        /// </summary>
        Required,
        /// <summary>
        /// The product is not required by the process and
        /// it's absence will not reduce the throughput to 
        /// 0 and may raise the throughput above 1.
        /// If the product is an input, when consumed it
        /// becomes it's failed products.
        /// If the product is a capital, it follows stardard
        /// capital logic for destruction.
        /// </summary>
        Optional,
        /// <summary>
        /// The product is consumed by the process but is
        /// transformed into it's failure products rather
        /// than the process outputs.
        /// This is meant for inputs and not capital.
        /// </summary>
        Consumed,
        /// <summary>
        /// The product does not increase if throughput 
        /// increases, it will always require the same
        /// amount. This often applies to Labor, but can
        /// apply to other things.
        /// </summary>
        Fixed
    }
}
