using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    public class PopulationCultureBreakdown
    {
        public int Id { get; set; }

        [Required]
        public virtual PopulationGroup Parent { get; set; }

        [Required]
        public virtual Culture Culture { get; set; }

        /// <summary>
        /// The member count of the parent population of this culture.
        /// </summary>
        [Required]
        public double Amount { get; set; }
    }
}
