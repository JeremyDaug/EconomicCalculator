﻿using EconomicCalculator.Enums;
using EconomicCalculator.Storage.Products.ProductTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Products
{
    /// <summary>
    /// Interface for the underlying product data.
    /// </summary>
    public interface IProduct
    {
        /// <summary>
        /// The Unique Id of the Product.
        /// </summary>
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

        /// <summary>
        /// The tags of the Product.
        /// Key is the ID of the tag,
        /// The list is the parameters given.
        /// If list is null then there are no parameters.
        /// </summary>
        //Dictionary<int, List<int>> Tags { get; }

        // TODO Technology Connection Placeholder.

        /// <summary>
        /// The average time for the product to break in days.
        /// -1 means it never breaks, 0 means it cannot be stored and breaksdown immediately.
        /// 1 means it can be stored but will breakdown immediately.
        /// </summary>
        //int MTTF { get; }

        /// <summary>
        /// The products this product fails into or is consumed into.
        /// The key is the <see cref="Id"/> of the product is fails into.
        /// The Value is the amount it fails into per unit of this product.
        /// </summary>
        //IReadOnlyDictionary<int, decimal> FailsInto { get; }

        /// <summary>
        /// If the product can be maintained.
        /// </summary>
        //bool Maintainable { get; }

        /// <summary>
        /// The products which maintain this product.
        /// The Key is the <see cref="Id"/> of the product which maintains this.
        /// The amount of that product needed per unit of this product.
        /// </summary>
        // TOOD, come back to this later. Maybe offload it into specific
        // processes.
        //IReadOnlyDictionary<int, decimal> Maintenance { get; }

        /// <summary>
        /// The Icon used by the product.
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// Gets the product's name in Product(Variant) format.
        /// </summary>
        /// <returns></returns>
        string GetName();
    }
}
