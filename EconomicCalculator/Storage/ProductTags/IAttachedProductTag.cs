using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.ProductTags
{
    /// <summary>
    /// Interface for Product Tag that's attached to a product.
    /// </summary>
    public interface IAttachedProductTag
    {
        /// <summary>
        /// The Tag attached to the product.
        /// </summary>
        ProductTag Tag { get; }

        /// <summary>
        /// List of parameter types.
        /// If empty, there are no expected parameters.
        /// </summary>
        IList<ParameterType> TagParameterTypes { get; }

        /// <summary>
        /// Indexor
        /// </summary>
        /// <param name="i">The index To Access</param>
        /// <returns>The Parameter at that index.</returns>
        /// <exception cref="IndexOutOfRangeException"/>
        object this[int i] { get; }

        /// <summary>
        /// To String form.
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
