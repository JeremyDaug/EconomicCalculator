using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Processes
{
    public enum ProcessPartTag
    {
        /// <summary>
        /// Input, consumed by the process.
        /// </summary>
        Input,
        /// <summary>
        /// Capital, used, but not consumed by the process.
        /// </summary>
        Capital,
        /// <summary>
        /// Output, a result of the process.
        /// </summary>
        Output
    }
}
