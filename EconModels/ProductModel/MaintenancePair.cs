using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.ProductModel
{
    public class MaintenancePair
    {
        // Parent Product
        [Required, Index("UniqueCoupling", 1, IsUnique = true)]
        public int SourceId { get; set; }

        /// <summary>
        /// What is doing the maintenance.
        /// </summary>
        public virtual Product Source { get; set; }

        // Child Product
        [Required, Index("UniqueCoupling", 2, IsUnique = true)]
        public int ResultId { get; set; }

        /// <summary>
        /// what is being maintained.
        /// </summary>
        public virtual Product Result { get; set; }

        [Required]
        public double Amount { get; set; }
    }
}
