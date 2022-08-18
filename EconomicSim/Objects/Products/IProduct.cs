﻿using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products.ProductTags;
using EconomicSim.Objects.Technology;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Products
{
    /// <summary>
    /// Product Read Only Interface
    /// </summary>
    [JsonConverter(typeof(ProductJsonConverter))]
    public interface IProduct
    {
        /// <summary>
        /// The ID of the product
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The Name of the product, cannot be empty.
        /// </summary>
        /// <remarks>
        /// Name and Variant Name should be a unique combo.
        /// </remarks>
        string Name { get; }

        /// <summary>
        /// The Variant Name of the product, may be empty.
        /// </summary>
        /// <remarks>
        /// Name and VariantName should be a unique combo.
        /// </remarks>
        string VariantName { get; }

        /// <summary>
        /// The name of the unit the product is measured in.
        /// </summary>
        string UnitName { get; }

        /// <summary>
        /// The quality of the product. Related to how luxurious or
        /// cheap it is seen as.
        /// </summary>
        int Quality { get; }

        /// <summary>
        /// The mass of the product in Kg.
        /// </summary>
        decimal Mass { get; }

        /// <summary>
        /// The space the object takes up in m^3.
        /// </summary>
        decimal Bulk { get; }

        /// <summary>
        /// Whether the product can be devided into decmial units.
        /// </summary>
        bool Fractional { get; }

        /// <summary>
        /// The location of the Product's Icon.
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// The tags of the product.
        /// </summary>
        IReadOnlyList<(ProductTag tag, Dictionary<string, object>? parameters)> ProductTags { get; }

        /// <summary>
        /// What wants the product satisfies by just owning.
        /// Item1 is the want it applies to
        /// Item2 is how much it gives (or takes)
        /// </summary>
        IReadOnlyDictionary<IWant, decimal> Wants { get; }

        /// <summary>
        /// Other processes related to this product.
        /// Use, Consumption, and Maintenance products for this
        /// product.
        /// </summary>
        IReadOnlyList<IProcess> ProductProcesses { get; }

        /// <summary>
        /// The failure Process of the product, may be empty.
        /// </summary>
        IProcess? FailureProcess { get; }

        /// <summary>
        /// The Use Processes of the Product
        /// </summary>
        IReadOnlyList<IProcess> UseProcesses { get; }

        /// <summary>
        /// The ways the product can be used.
        /// </summary>
        IReadOnlyList<IProcess> ConsumptionProcesses { get; }

        /// <summary>
        /// They ways the product can be maintained.
        /// </summary>
        IReadOnlyList<IProcess> MaintenanceProcesses { get; }

        /// <summary>
        /// The tech required to know/build this product.
        /// May be null.
        /// </summary>
        ITechnology? TechRequirement { get; }

        /// <summary>
        /// Get's the proper Name(variant) name of the Product.
        /// </summary>
        /// <returns></returns>
        string GetName();
    }
}
