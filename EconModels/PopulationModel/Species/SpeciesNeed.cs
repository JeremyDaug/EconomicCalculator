using EconModels.ProductModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    public class SpeciesNeed
    {
        [Required]
        [DisplayName("Species Id")]
        public int SpeciesId { get; set; }

        [ForeignKey("SpeciesId")]
        [DisplayName("Species")]
        public virtual Species Species { get; set; }

        [Required]
        [DisplayName("Need Id")]
        public int NeedId { get; set; }

        [ForeignKey("NeedId")]
        [DisplayName("Need")]
        public Product Need { get; set; }

        [Required, Range(0, double.MaxValue)]
        [DisplayName("Amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// The Tag(s) of the anathema, describing it's more advanced effects.
        /// </summary>
        [Required, StringLength(30)]
        [DisplayName("Tag")]
        public string Tag { get; set; }
    }
}