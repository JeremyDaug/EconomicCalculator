using EconomicCalculator.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Processes.ProcessTags
{
    /// <summary>
    /// Interface for Attached Process Tags.
    /// </summary>
    public interface IAttachedProcessTag
    {
        /// <summary>
        /// The Tag Attached.
        /// </summary>
        ProcessTag Tag { get; }

        /// <summary>
        /// The Tag's Parameter Types.
        /// </summary>
        IList<ParameterType> TagParameterTypes { get; }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="i">The Desired index.</param>
        /// <returns>The data of that a parameter at that index.</returns>
        object this[int i] { get; }

        /// <summary>
        /// To String Form.
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
