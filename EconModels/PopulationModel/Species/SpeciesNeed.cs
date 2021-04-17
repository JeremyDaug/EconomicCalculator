using EconModels.ProductModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    public class SpeciesNeed
    {
        [Required]
        public int SpeciesId { get; set; }

        [Required, ForeignKey("SpeciesId")]
        public virtual Species Species { get; set; }

        [Required]
        public int NeedId { get; set; }

        [ForeignKey("NeedId")]
        public Product Need { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        /// <summary>
        /// The Tag(s) of the anathema, describing it's more advanced effects.
        /// </summary>
        [Required, StringLength(30)]
        public string Tag { get; set; }
    }
}