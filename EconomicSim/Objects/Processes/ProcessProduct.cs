using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes.ProductionTags;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Processes
{
    /// <summary>
    /// Process Product Data Class
    /// </summary>
    [JsonConverter(typeof(ProcessProductJsonConverter))]
    public class ProcessProduct : IProcessProduct
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProcessProduct()
        {
            TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>();
        }

        /// <summary>
        /// The product in question.
        /// </summary>
        public IProduct Product { get; set; }

        /// <summary>
        /// The amount of it expected.
        /// </summary>
        public decimal Amount { get; set; }

        // TODO update this to use a class instead of a list for more ease.
        /// <summary>
        /// The Tags and respective data they need.
        /// </summary>
        public List<(ProductionTag tag, Dictionary<string, object> parameters)> TagData { get; set; } 
        IReadOnlyList<(ProductionTag tag, Dictionary<string, object> parameters)> IProcessProduct.TagData => TagData;
        
        /// <summary>
        /// The part it belongs to.
        /// </summary>
        public ProcessPartTag Part { get; set; }

        /// <summary>
        /// Checks if the Process Product contains the selected tag.
        /// </summary>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>True if found, false otherwise.</returns>
        public bool ContainsTag(ProductionTag tag)
        {
            return TagData.Any(x => x.tag == tag);
        }
    }
}
