using EconModels.ProductModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    public class SpeciesAnathema
    {
        [Required]
        [DisplayName("Species Id")]
        public int SpeciesId { get; set; }

        [ForeignKey("SpeciesId")]
        [DisplayName("Species")]
        public virtual Species Species { get; set; }

        [Required]
        [DisplayName("Anathema Id")]
        public int AnathemaId { get; set; }

        [ForeignKey("AnathemaId")]
        [DisplayName("Anathema Product")]
        public Product Anathema { get; set; }

        [Required, Range(0, double.MaxValue)]
        [DisplayName("Amount")]
        public virtual decimal Amount { get; set; }

        /// <summary>
        /// The Tag(s) of the anathema, describing it's more advanced effects.
        /// </summary>
        [Required, StringLength(30)]
        [DisplayName("Tag")]
        public string Tag { get; set; }
    }
}