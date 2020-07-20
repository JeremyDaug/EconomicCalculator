using EconomicCalculator.Intermediaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage
{
    /// <summary>
    /// TODO: This is on hold until I need it and/or my sanity needs a break.
    /// </summary>
    public class ProductStorage
    {
        /// <summary>
        /// The products stored.
        /// </summary>
        private IList<IProduct> Products;

        /// <summary>
        /// The Amount of each product.
        /// </summary>
        private IDictionary<string, double> ProductCounts;

        /// <summary>
        /// Checks that a product exists.
        /// </summary>
        /// <param name="productName">The name of the product.</param>
        /// <returns>True if found, false otherwise.</returns>
        public bool Contains(string productName) => Products.Any(x => x.Name == productName);

        public bool Contains(string productName, string variant) 
            => Products.Any(x => x.Name == productName &&
                                 x.VariantName == variant);
    }
}
