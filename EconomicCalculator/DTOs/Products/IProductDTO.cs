using EconomicCalculator.DTOs.Processes;
using EconomicCalculator.DTOs.Products.ProductTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Products
{
    /// <summary>
    /// Interface for the underlying product data.
    /// </summary>
    [Obsolete]
    public interface IProductDTO
    {
        /// <summary>
        /// The Unique Id of the Product.
        /// </summary>
        [JsonIgnore]
        int Id { get; }

        /// <summary>
        /// The Name of the Product, cannot be empty.
        /// </summary>
        /// <remarks>
        /// Name and VariantName should be a unique combo among all products.
        /// </remarks>
        string Name { get; }

        /// <summary>
        /// Variant Name of the product. 
        /// </summary>
        /// <remarks>
        /// Name and VariantName should be a unique combo among all products.
        /// </remarks>
        string VariantName { get; }

        /// <summary>
        /// The name of the unit the product is measured in.
        /// </summary>
        string UnitName { get; }

        /// <summary>
        /// The Quality of the product, the lower the value, the lower the quality and
        /// vice versa.
        /// </summary>
        int Quality { get; }

        // Default Price? Skip for now, may not need.

        /// <summary>
        /// The Mass of the product in Kg.
        /// </summary>
        decimal Mass { get; }

        /// <summary>
        /// The space the product takes up in m^3.
        /// </summary>
        decimal Bulk { get; }

        // No Product Tags, not needed anymore, covered by Tags.

        /// <summary>
        /// If the product can be divided into subunit sizes.
        /// </summary>
        bool Fractional { get; }

        /// <summary>
        /// The wants the product satisfies. The Key is the Want Id.
        /// The Value is the amount satisfied per unit.
        /// </summary>
        [JsonIgnore]
        Dictionary<int, decimal> Wants { get; set; }

        /// <summary>
        /// Alternative storage method for wants.
        /// </summary>
        List<string> WantStrings { get; set; }

        /// <summary>
        /// A Helper to view the want strings.
        /// Ordered in the same way as <seealso cref="WantStrings"/>.
        /// </summary>
        [JsonIgnore]
        string WantString { get; }

        /// <summary>
        /// String form of our tags.
        /// </summary>
        List<string> TagStrings { get; set; }

        /// <summary>
        /// Product Tags in proper form.
        /// </summary>
        [JsonIgnore]
        List<IAttachedProductTag> Tags { get; }

        /// <summary>
        /// String form of all our tags.
        /// </summary>
        [JsonIgnore]
        string TagString { get; }

        // TODO Technology Connection Placeholder.

        /// <summary>
        /// The Icon used by the product.
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// The failure process of the product
        /// </summary>
        [JsonIgnore]
        IProcessDTO Failure { get; }

        /// <summary>
        /// Gets the product's name in Product(Variant) format.
        /// </summary>
        /// <returns></returns>
        string GetName();

        /// <summary>
        /// Check if a product has a tag or not.
        /// </summary>
        /// <param name="tag">The tag we want to check for.</param>
        /// <returns>True if found, false otherwise.</returns>
        bool ContainsTag(ProductTag tag);
    }
}
