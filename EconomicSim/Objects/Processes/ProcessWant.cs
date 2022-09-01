using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes.ProductionTags;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Processes
{
    /// <summary>
    /// Process Want Data
    /// </summary>
    [JsonConverter(typeof(ProcessWantJsonConverter))]
    public class ProcessWant : IProcessWant
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProcessWant()
        {
            TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>();
        }

        /// <summary>
        /// The want in question.
        /// </summary>
        public IWant Want { get; set; }

        /// <summary>
        /// The amount of the want desired or produced.
        /// </summary>
        public decimal Amount { get; set; }

        // TODO update this to use a class instead of a list for more ease.
        /// <summary>
        /// The Read Only Tag Data
        /// </summary>
        public List<(ProductionTag tag, Dictionary<string, object> parameters)> TagData { get; set; }
        IReadOnlyList<(ProductionTag tag, Dictionary<string, object> parameters)> IProcessWant.TagData => TagData;

        /// <summary>
        /// The part the want belongs to.
        /// </summary>
        public ProcessPartTag Part { get; set; }

        /// <summary>
        /// Checks if the Process Want contains the selected tag.
        /// </summary>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>True if found, false otherwise.</returns>
        public bool ContainsTag(ProductionTag tag)
        {
            return TagData.Any(x => x.tag == tag);
        }
    }
}
