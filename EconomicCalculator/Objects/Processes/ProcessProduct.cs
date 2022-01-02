using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Objects.Processes.ProductionTags;
using EconomicCalculator.Objects.Products;

namespace EconomicCalculator.Objects.Processes
{
    /// <summary>
    /// Process Product Data Class
    /// </summary>
    internal class ProcessProduct : IProcessProduct
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProcessProduct()
        {
            tagData = new List<IProductionTagData>();
        }

        /// <summary>
        /// The product in question.
        /// </summary>
        public IProduct Product { get; set; }

        /// <summary>
        /// The amount of it expected.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The Tags and respective data they need.
        /// </summary>
        public IReadOnlyList<IProductionTagData> TagData => tagData;
        
        /// <summary>
        /// Editable data for tags.
        /// </summary>
        public List<IProductionTagData> tagData { get; set; }

        /// <summary>
        /// The part it belongs to.
        /// </summary>
        public ProcessPartTag Part { get; set; }
    }
}
