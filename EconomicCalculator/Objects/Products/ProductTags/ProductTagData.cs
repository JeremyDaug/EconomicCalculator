using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Products.ProductTags
{
    /// <summary>
    /// Product Tag Data Storage
    /// </summary>
    public class ProductTagData : IProductTagData
    {
        /// <summary>
        /// The Tag and it's data.
        /// </summary>
        /// <param name="tag">The tag attached.</param>
        /// <param name="data">The Data attached to that tag.</param>
        public ProductTagData(ProductTag tag, List<object> data)
        {
            Tag = tag;
            this.data = data;
        }

        private List<object> data;

        /// <summary>
        /// The attached tag of the product.
        /// </summary>
        public ProductTag Tag { get; set; }

        /// <summary>
        /// Indexor
        /// </summary>
        /// <param name="i">In index to access</param>
        /// <returns>The parameter at that index.</returns>
        public object this[int i]
        {
            get
            {
                return data[i];
            }
        }
    }
}
