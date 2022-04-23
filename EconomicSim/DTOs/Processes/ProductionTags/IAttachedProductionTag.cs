using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicSim.DTOs.Enums;

namespace EconomicSim.DTOs.Processes.ProductionTags
{
    /// <summary>
    /// Interface for Product Tags Attached to a product.
    /// </summary>
    public interface IAttachedProductionTag
    {
        /// <summary>
        /// The Tag Attached.
        /// </summary>
        ProductionTag Tag { get; }

        /// <summary>
        /// The Tag's parameter types.
        /// </summary>
        IList<ParameterType> TagParameterTypes { get; }

        /// <summary>
        /// The Tag's Indexer
        /// </summary>
        /// <param name="i">The index to access.</param>
        /// <returns></returns>
        object this[int i] { get; }

        /// <summary>
        /// To String Form
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
