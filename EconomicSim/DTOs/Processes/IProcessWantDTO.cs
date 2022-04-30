using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicSim.DTOs.Processes.ProductionTags;

namespace EconomicSim.DTOs.Processes
{
    /// <summary>
    /// Wants desired or produced by a process
    /// </summary>
    [Obsolete]
    public interface IProcessWantDTO
    {
        /// <summary>
        /// The want's name.
        /// </summary>
        string WantName { get; }

        /// <summary>
        /// The Want it desires.
        /// </summary>
        [JsonIgnore]
        int WantId { get; }

        /// <summary>
        /// How much of that want it desires.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// String form of our tags.
        /// </summary>
        List<string> TagStrings { get; }

        /// <summary>
        /// What tags this Want has for the production process.
        /// </summary>
        [JsonIgnore]
        List<IAttachedProductionTag> Tags { get; }
        
        /// <summary>
        /// String form of all our tags.
        /// </summary>
        [JsonIgnore]
        string TagString { get; }

        /// <summary>
        /// Sets tags of the process want from existing tag strings.
        /// </summary>
        void SetTagsFromStrings();
    }
}
