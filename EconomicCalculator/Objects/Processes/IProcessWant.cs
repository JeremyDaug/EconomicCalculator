using EconomicCalculator.Helpers;
using EconomicCalculator.Objects.Processes.ProductionTags;
using EconomicCalculator.Objects.Wants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Processes
{
    /// <summary>
    /// Readonly Process input/capital/output want interface.
    /// </summary>
    public interface IProcessWant
    {
        /// <summary>
        /// The desired or recieved Want.
        /// </summary>
        IWant Want { get; }

        /// <summary>
        /// The amount of the want desired.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// The Tag Data of the want.
        /// </summary>
        IReadOnlyList<ITagData<ProductionTag>> TagData { get; }

        /// <summary>
        /// The Part of the process it belongs to.
        /// </summary>
        ProcessPartTag Part { get; }
    }
}
