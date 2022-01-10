using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Helpers;
using EconomicCalculator.Objects.Processes.ProductionTags;
using EconomicCalculator.Objects.Wants;

namespace EconomicCalculator.Objects.Processes
{
    /// <summary>
    /// Process Want Data
    /// </summary>
    internal class ProcessWant : IProcessWant
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProcessWant()
        {
            tagData = new List<ITagData<ProductionTag>>();
        }

        /// <summary>
        /// The want in question.
        /// </summary>
        public IWant Want { get; set; }

        /// <summary>
        /// The amount of the want desired or produced.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The Read Only Tag Data
        /// </summary>
        public IReadOnlyList<ITagData<ProductionTag>> TagData => tagData;

        /// <summary>
        /// The editable tag data.
        /// </summary>
        public List<ITagData<ProductionTag>> tagData { get; set; }

        /// <summary>
        /// The part the want belongs to.
        /// </summary>
        public ProcessPartTag Part { get; set; }
    }
}
