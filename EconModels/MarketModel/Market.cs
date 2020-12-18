using EconModels.PopulationModel;
using EconModels.ProductModel;
using EconModels.TerritoryModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.MarketModel
{
    public class Market
    {
        public Market()
        {
            PopulationGroups = new List<PopulationGroup>();
            ProductPrices = new List<ProductPrices>();
            ValidCurrencies = new List<Product>();
        }

        public int Id { get; set; }

        [Required, StringLength(40, MinimumLength = 3)]
        public string Name { get; set; }

        // Total Population, 
        // Summed from population Groups

        // Territory Placeholder
        public virtual Territory Territory { get; set; }

        // Barter Bool, not stored in DB as it should be build wide.

        // Population Groups
        public virtual ICollection<PopulationGroup> PopulationGroups { get; set; }

        // Product Prices
        public virtual ICollection<ProductPrices> ProductPrices { get; set; }

        // Valid Currencies, price is part of product prices.
        public virtual ICollection<Product> ValidCurrencies { get; set; }
    }
}
 