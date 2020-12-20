using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.ProductModel
{
    public class MaintenancePair
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
}
