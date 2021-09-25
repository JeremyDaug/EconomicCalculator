using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Processes
{
    /// <summary>
    /// Product desired or produced by a process.
    /// </summary>
    internal class ProcessProduct : IProcessProduct
    {
        /// <summary>
        /// The name of the product desired.
        /// </summary>
        public string ProductName => Manager.Instance.Products[ProductId].GetName();

        /// <summary>
        /// The product it desires.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// How much it desires.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// What tags this product has for the production process.
        /// </summary>
        public IList<IAttachedProductionTag> Tags { get; set; }
    }
}
