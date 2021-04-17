using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    public class PoliticalBreakdown
    {
        [Required]
        public int ParentId { get; set; }

        [Required, ForeignKey("ParentId")]
        public virtual PopulationGroup Parent { get; set; }

        [Required]
        public int PoliticalGroupId { get; set; }

        [Required, ForeignKey("PoliticalGroupId")]
        public virtual PoliticalGroup PoliticalGroup { get; set; }

        [Required, Range(0, 1)]
        public double Percent { get; set; }
    }
}