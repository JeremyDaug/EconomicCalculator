﻿using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes.ProductionTags;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Processes
{
    /// <summary>
    /// A product used in or outputted from a process.
    /// </summary>
    [JsonConverter(typeof(ProcessProductJsonConverter))]
    public interface IProcessProduct
    {
        /// <summary>
        /// The Product
        /// </summary>
        IProduct Product { get; }

        /// <summary>
        /// How much of the product is requested.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// The Tags and related data needed for the product.
        /// </summary>
        IReadOnlyList<(ProductionTag tag, Dictionary<string, object> parameters)> TagData { get; }

        /// <summary>
        /// The Part of the process it belongs to.
        /// </summary>
        ProcessPartTag Part { get; }
    }
}
