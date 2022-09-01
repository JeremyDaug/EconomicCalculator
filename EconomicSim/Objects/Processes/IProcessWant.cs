using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes.ProductionTags;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Processes
{
    /// <summary>
    /// Readonly Process input/capital/output want interface.
    /// </summary>
    [JsonConverter(typeof(ProcessWantJsonConverter))]
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
        IReadOnlyList<(ProductionTag tag, Dictionary<string, object> parameters)> TagData { get; }

        /// <summary>
        /// The Part of the process it belongs to.
        /// </summary>
        ProcessPartTag Part { get; }

        /// <summary>
        /// Checks if the Process Want contains the selected tag.
        /// </summary>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>True if found, false otherwise.</returns>
        bool ContainsTag(ProductionTag tag);
    }
}
