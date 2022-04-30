using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicSim.Helpers;
using EconomicSim.Objects.Processes.ProductionTags;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Processes
{
    /// <summary>
    /// Process Product Data Class
    /// </summary>
    [JsonConverter(typeof(ProcessProductJsonConverter))]
    internal class ProcessProduct : IProcessProduct
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProcessProduct()
        {
            TagData = new List<(ProductionTag tag, Dictionary<string, object> properties)>();
        }

        /// <summary>
        /// The product in question.
        /// </summary>
        public IProduct Product { get; set; }

        /// <summary>
        /// The amount of it expected.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The Tags and respective data they need.
        /// </summary>
        public List<(ProductionTag tag, Dictionary<string, object> properties)> TagData { get; set; } 
        IReadOnlyList<(ProductionTag tag, Dictionary<string, object> properties)> IProcessProduct.TagData => TagData;
        
        /// <summary>
        /// The part it belongs to.
        /// </summary>
        public ProcessPartTag Part { get; set; }
    }
}
