﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicSim.Helpers;
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
        IReadOnlyList<(ProductionTag tag, Dictionary<string, object> properties)> TagData { get; }

        /// <summary>
        /// The Part of the process it belongs to.
        /// </summary>
        ProcessPartTag Part { get; }
    }
}