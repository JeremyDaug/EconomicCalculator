using EconModels.ProductModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    public class SpeciesAnathema
    {
        [Required]
        public int SpeciesId { get; set; }

        [Required, ForeignKey("SpeciesId")]
        public virtual Species Species { get; set; }

        [Required]
        public int AnathemaId { get; set; }

        [Required, ForeignKey("AnathemaId")]
        public Product Anathema { get; set; }

        [Required, Range(0, double.MaxValue)]
        public virtual decimal Amount { get; set; }

        /// <summary>
        /// The Tag(s) of the anathema, describing it's more advanced effects.
        /// </summary>
        [Required, StringLength(30)]
        public string Tag { get; set; }
    }
}