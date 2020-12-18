using EconModels.ProductModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.MarketModel
{
    public class ProductPrices
    {
        public int Id { get; set; }

        [Required, Index("UniqueCoupling", 1, IsUnique = true)]
        public int MarketId { get; set; }

        public virtual Market Market { get; set; }

        [Required, Index("UniqueCoupling", 2, IsUnique = true)]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        // The price of the product in the market.
        [Required]
        public decimal MarketPrice { get; set; }
    }
}
