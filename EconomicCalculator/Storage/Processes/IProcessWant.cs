using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Processes
{
    /// <summary>
    /// Wants desired or produced by a process
    /// </summary>
    public interface IProcessWant
    {
        /// <summary>
        /// The want's name.
        /// </summary>
        string WantName { get; }

        /// <summary>
        /// The Want it desires.
        /// </summary>
        int WantId { get; }

        /// <summary>
        /// How much of that want it desires.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// What tags this Want has for the production process.
        /// </summary>
        IList<IAttachedProductionTag> Tags { get; }
        
    }
}
