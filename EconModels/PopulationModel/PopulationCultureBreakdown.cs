using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [ForeignKey("ParentId")]
        [DisplayName("Parent Pop")]
        public virtual PopulationGroup Parent { get; set; }

        [Required]
        public int CultureId { get; set; }

        [ForeignKey("CultureId")]
        [DisplayName("Culture")]
        public virtual Culture Culture { get; set; }

        /// <summary>
        /// The member count of the parent population of this culture.
        /// </summary>
        [Required, Range(0, 1)]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double Percent { get; set; }
    }
}