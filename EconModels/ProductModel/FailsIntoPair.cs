using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.ProductModel
{
    public class FailsIntoPair
    {
        // Parent Product
        [Required, Index("UniqueCoupling", 1, IsUnique = true)]
        [DisplayName("Source Id")]
        public int SourceId { get; set; }

        /// <summary>
        /// What is failing.
        /// </summary>
        [DisplayName("Source")]
        public virtual Product Source { get; set; }

        // Child Product
        [Required, Index("UniqueCoupling", 2, IsUnique = true)]
        [DisplayName("Result Id")]
        public int ResultId { get; set; }

        /// <summary>
        /// what it fails into
        /// </summary>
        [DisplayName("Result")]
        public virtual Product Result { get; set; }

        /// <summary>
        /// How many per unit of the source is produced.
        /// </summary>
        [Required]
        [DisplayName("Unit Conversion Rate")]
        public double Amount { get; set; }
    }
}
