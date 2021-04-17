using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    public class SpeciesWant
    {
        [Required]
        public int SpeciesId { get; set; }

        [ForeignKey("SpeciesId")]
        public virtual Species Species { get; set; }

        [Required, StringLength(20)]
        public string Want { get; set; }

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