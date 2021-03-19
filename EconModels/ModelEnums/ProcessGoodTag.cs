using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.ModelEnums
{
    /// <summary>
    /// Various Tags for the inputs, capital, and outputs of
    /// a process.
    /// </summary>
    [Flags]
    public enum ProcessGoodTag
    {
        /// <summary>
        /// Default Tag, the product is required and the 
        /// ratio of it's satisfaction reduces the throughput
        /// of the entire process. It is consumed by the 
        /// process and does not produce it's failure 
        /// products.
        /// </summary>
        Required = 0,
        /// <summary>
        /// The product is not required by the process and
        /// it's absence will not reduce the throughput to 
        /// 0 and may raise the throughput above 1.
        /// If the product is an input, when consumed it
        /// becomes it's failed products.
        /// If the product is a capital, it follows stardard
        /// capital logic for destruction.
        /// </summary>
        Optional = 1,
        /// <summary>
        /// The product is consumed by the process but is
        /// transformed into it's failure products rather
        /// than the process outputs.
        /// This is meant for inputs and not capital.
        /// </summary>
        Consumed = 2,
        /// <summary>
        /// The product does not increase if throughput 
        /// increases, it will always require the same
        /// amount. This often applies to Labor, but can
        /// apply to other things.
        /// </summary>
        Fixed = 4,
        /// <summary>
        /// An investment product is input only once at the
        /// start of a process. This is primarily used for
        /// long term Processes like farming or loans.
        /// </summary>
        Investment = 8,
        /// <summary>
        /// This consumed good is optional.
        /// </summary>
        OptionalConsumed = Optional | Consumed,
        /// <summary>
        /// This fixed Good is optional, it does not grow with
        /// other optional inputs (not that it would anyway).
        /// </summary>
        FixedOptional = Fixed | Optional,
        /// <summary>
        /// This investment is optional for the process, and
        /// instead of being considered an output, it's instead
        /// consumed.
        /// </summary>
        OptionalInvestment = Optional | Investment,
        /// <summary>
        /// This Investment is required, but consumed.
        /// </summary>
        ConsumedInvestment = Consumed | Investment,
        /// <summary>
        /// This investment is fixed, even if optional goods exist
        /// to increase throughput.
        /// </summary>
        FixedInvestment = Fixed | Investment
    }
}
