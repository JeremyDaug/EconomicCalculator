using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicSim.Helpers;
using EconomicSim.Objects.Processes.ProductionTags;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Processes
{
    /// <summary>
    /// Process Want Data
    /// </summary>
    [JsonConverter(typeof(ProcessWantJsonConverter))]
    internal class ProcessWant : IProcessWant
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProcessWant()
        {
            TagData = new List<(ProductionTag tag, Dictionary<string, object> properties)>();
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
        public List<(ProductionTag tag, Dictionary<string, object> properties)> TagData { get; set; }
        IReadOnlyList<(ProductionTag tag, Dictionary<string, object> properties)> IProcessWant.TagData => TagData;

        /// <summary>
        /// The part the want belongs to.
        /// </summary>
        public ProcessPartTag Part { get; set; }
    }
}
