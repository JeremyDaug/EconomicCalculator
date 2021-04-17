using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    public class SpeciesAversion
    {
        [Required]
        public int SpeciesId { get; set; }

        [Required, ForeignKey("SpeciesId")]
        public virtual Species Species { get; set; }

        [StringLength(20)]
        public string Aversion { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        /// <summary>
        /// The tag(s) of the aversion, describing it's more 
        /// advanced effects.
        /// </summary>
        [Required, StringLength(30)]
        public string Tag { get; set; }
    }
}