using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.ProductModel
{
    public abstract class ProductToProductPair
    {
        public int Id { get; set; }

        // Parent Product
        [Required, Index("UniqueCoupling", 1, IsUnique = true)]
        public int SourceId { get; set; }

        public virtual Product Source { get; set; }

        // Child Product
        [Required, Index("UniqueCoupling", 2, IsUnique = true)]
        public int ResultId { get; set; }

        public virtual Product Result { get; set; }

        [Required]
        public double Amount { get; set; }
    }

    public class FailsIntoPair : ProductToProductPair { }

    public class MaintenancePair : ProductToProductPair { }
}
