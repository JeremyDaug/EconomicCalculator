using EconDTOs.DTOs.Processes.ProductionTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconDTOs.DTOs.Processes
{
    /// <summary>
    /// Product desired or produced by a process.
    /// </summary>
    public interface IProcessProduct
    {
        /// <summary>
        /// The name of the product desired.
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// The product it desires.
        /// </summary>
        [JsonIgnore]
        int ProductId { get; }

        /// <summary>
        /// How much it desires.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// String form of our tags
        /// </summary>
        List<string> TagStrings { get; }

        /// <summary>
        /// What tags this product has for the production process.
        /// </summary>
        [JsonIgnore]
        List<IAttachedProductionTag> Tags { get; }

        /// <summary>
        /// String form of all our tags
        /// </summary>
        [JsonIgnore]
        string TagString { get; }

        /// <summary>
        /// Set Tags from strings.
        /// </summary>
        void SetTagsFromStrings();
    }
}
