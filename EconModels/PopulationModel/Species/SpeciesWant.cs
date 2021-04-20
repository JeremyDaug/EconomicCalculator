using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    public class SpeciesWant
    {
        [Required]
        [DisplayName("Species Id")]
        public int SpeciesId { get; set; }

        [ForeignKey("SpeciesId")]
        [DisplayName("Species")]
        public virtual Species Species { get; set; }

        [Required, StringLength(20)]
        [DisplayName("Want")]
        public string Want { get; set; }

        [Required, Range(0, double.MaxValue)]
        [DisplayName("Amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// The tag(s) of the aversion, describing it's more 
        /// advanced effects.
        /// </summary>
        [Required, StringLength(30)]
        [DisplayName("Tag")]
        public string Tag { get; set; }
    }
}