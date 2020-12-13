using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModels
{
    public class PopulationCultureBreakdown
    {
        public int Id { get; set; }

        [Required]
        public virtual PopulationGroup Parent { get; set; }

        [Required]
        public virtual Culture Culture { get; set; }

        /// <summary>
        /// The total population of the 
        /// </summary>
        [Required]
        public double Amount { get; set; }
    }
}
