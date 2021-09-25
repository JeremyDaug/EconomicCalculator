﻿using EconomicCalculator.Storage.Processes.ProductionTags;
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
    public interface IProcessProduct
    {
        /// <summary>
        /// The name of the product desired.
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// The product it desires.
        /// </summary>
        int ProductId { get; }

        /// <summary>
        /// How much it desires.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// What tags this product has for the production process.
        /// </summary>
        IList<IAttachedProductionTag> Tags { get; }
    }
}
