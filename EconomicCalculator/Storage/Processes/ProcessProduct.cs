﻿using EconomicCalculator.Storage.Processes.ProductionTags;
using EconomicCalculator.Storage.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Processes
{
    /// <summary>
    /// Product desired or produced by a process.
    /// </summary>
    public class ProcessProduct : IProcessProduct
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProcessProduct()
        {
            TagStrings = new List<string>();
            Tags = new List<IAttachedProductionTag>();
        }

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
        public decimal Amount { get; set; }

        /// <summary>
        /// String form of our tags
        /// </summary>
        public List<string> TagStrings { get; set; }

        /// <summary>
        /// What tags this product has for the production process.
        /// </summary>
        [JsonIgnore]
        public List<IAttachedProductionTag> Tags { get; set; }

        /// <summary>
        /// String form of all our tags
        /// </summary>
        [JsonIgnore]
        public string TagString
        {
            get
            {
                var result = "";

                foreach (var tag in TagStrings)
                    result += tag + ";";

                result = result.TrimEnd(';');

                return result;
            }
        }

        public override string ToString()
        {
            var result = 
                String.Format("{0} <{1}> -> {2} {3}",
                    ProductName, TagString, Amount,
                    Manager.Instance.Products[ProductId].UnitName);

            return result;
        }
    }
}