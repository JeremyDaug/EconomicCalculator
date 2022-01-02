using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Processes.ProductionTags
{
    internal class ProductionTagData : IProductionTagData
    {
        public ProductionTagData(ProductionTag tag, List<object> data)
        {
            Tag = tag;
            this.data = data;
        }

        private List<object> data;

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

        /// <summary>
        /// The production tag of the product or want.
        /// </summary>
        public ProductionTag Tag { get; set; }
    }
}
