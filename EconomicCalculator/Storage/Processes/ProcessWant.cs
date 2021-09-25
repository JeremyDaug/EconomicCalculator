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
    public class ProcessWant : IProcessWant
    {
        /// <summary>
        /// The want's name.
        /// </summary>
        public string WantName => Manager.Instance.Wants[WantId].Name;

        /// <summary>
        /// The Want it desires.
        /// </summary>
        public int WantId { get; set;  }

        /// <summary>
        /// How much of that want it desires.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// What tags this Want has for the production process.
        /// </summary>
        public IList<IAttachedProductionTag> Tags { get; set; }
    }
}
