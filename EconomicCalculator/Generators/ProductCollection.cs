using EconomicCalculator.Intermediaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
{
    public class ProductCollection
    {
        IList<IProductCount> Products;

        IProductCount ByName(string Name)
        {
            return Products.Single(x => x.Product.Name == Name);
        }

        public IProductCount GetProductCount(IProduct product)
        {

        }
    }
}
