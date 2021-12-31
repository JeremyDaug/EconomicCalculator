using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Products.ProductTags
{
    /// <summary>
    /// Read only product Tag Data Storage Interface.
    /// </summary>
    public interface IProductTagData
    {
        /// <summary>
        /// The Tag attached to the product.
        /// </summary>
        ProductTag Tag { get; }

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
