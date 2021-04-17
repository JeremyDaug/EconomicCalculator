using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    public class CultureBreakdown
    {
        [Required]
        public int ParentId { get; set; }

        [Required, ForeignKey("ParentId")]
        public virtual PopulationGroup Parent { get; set; }

        [Required]
        public int CultureId { get; set; }

        [Required, ForeignKey("CultureId")]
        public virtual Culture Culture { get; set; }

        /// <summary>
        /// The member count of the parent population of this culture.
        /// </summary>
        [Required, Range(0, 1)]
        public double Percent { get; set; }
    }
}