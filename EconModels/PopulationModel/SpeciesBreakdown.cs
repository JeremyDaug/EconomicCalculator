using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    /// <summary>
    /// Rows for breakdown of species in each group.
    /// </summary>
    public class SpeciesBreakdown
    {
        [Required]
        public int ParentId { get; set; }

        [Required, ForeignKey("ParentId")]
        public virtual PopulationGroup Parent { get; set; }

        [Required]
        public int SpeciesId { get; set; }

        [Required, ForeignKey("SpeciesId")]
        public virtual Species Species { get; set; }

        [Required, Range(0, 1)]
        public double Percent { get; set; }
    }
}