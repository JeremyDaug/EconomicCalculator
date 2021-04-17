using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    public class SpeciesTag
    {
        /// <summary>
        /// The Species this tag is attached to.
        /// </summary>
        [Required]
        public int SpeciesId { get; set; }

        /// <summary>
        /// The Species Attached to.
        /// </summary>
        [Required, ForeignKey("SpeciesId")]
        public virtual Species Species { get; set; }

        /// <summary>
        /// The tag of the species with data inside.
        /// </summary>
        [Required, StringLength(30)]
        public string Tag { get; set; }
    }
}