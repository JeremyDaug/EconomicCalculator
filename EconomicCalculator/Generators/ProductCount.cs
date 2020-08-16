using EconomicCalculator.Intermediaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
{
    public class ProductCount : IProductCount
    {
        /// <summary>
        /// The Product being counted.
        /// </summary>
        public IProduct Product { get; set; }

        /// <summary>
        /// The number of the product.
        /// </summary>
        public double Count { get; set; }
    }
}
